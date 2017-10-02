/*
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution. 

The Eclipse Public License is available at 
   http://www.eclipse.org/legal/epl-v10.html
and the Eclipse Distribution License is available at 
   http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
*/

using System;
using System.Net;
using System.Threading;
using LagoVista.MQTT.Core.Exceptions;
using LagoVista.MQTT.Core.Messages;
using LagoVista.MQTT.Core.Session;
using LagoVista.MQTT.Core.Utility;
using LagoVista.MQTT.Core.Internal;
using System.Linq;

// alias needed due to Microsoft.SPOT.Trace in .Net Micro Framework
// (it's ambiguos with LagoVista.MQTT.Core.Utility.Trace)
using MqttUtility = LagoVista.MQTT.Core.Utility;
using System.Threading.Tasks;
using System.Collections.Generic;
using LagoVista.Client.Core.Net;
using LagoVista.Core.Networking.Interfaces;
using System.Net.Sockets;

namespace LagoVista.MQTT.Core
{
    /// <summary>
    /// MQTT Client
    /// </summary>
    public class MqttClient
    {
        /// <summary>
        /// Delagate that defines event handler for PUBLISH message received
        /// </summary>
        public delegate void MqttMsgPublishEventHandler(object sender, MqttMsgPublishEventArgs e);

        /// <summary>
        /// Delegate that defines event handler for published message
        /// </summary>
        public delegate void MqttMsgPublishedEventHandler(object sender, MqttMsgPublishedEventArgs e);

        /// <summary>
        /// Delagate that defines event handler for subscribed topic
        /// </summary>
        public delegate void MqttMsgSubscribedEventHandler(object sender, MqttMsgSubscribedEventArgs e);

        /// <summary>
        /// Delagate that defines event handler for unsubscribed topic
        /// </summary>
        public delegate void MqttMsgUnsubscribedEventHandler(object sender, MqttMsgUnsubscribedEventArgs e);


        /// <summary>
        /// Delegate that defines event handler for cliet/peer disconnection
        /// </summary>
        public delegate void ConnectionClosedEventHandler(object sender, EventArgs e);

        // broker hostname (or ip address) and port
        string _brokerHostName;
        int _brokerPort;

        // running status of threads
        bool _isRunning;
        // event for raising received message event
        AutoResetEvent _receiveEventWaitHandle;

        // event for starting process inflight queue asynchronously
        AutoResetEvent _inflightWaitHandle;

        // event for signaling synchronous receive
        AutoResetEvent _syncEndReceiving;
        // message received
        MqttMsgBase _msgReceived;

        // exeption thrown during receiving
        Exception _exReceiving;

        // keep alive period (in ms)
        private int _keepAlivePeriod;
        // events for signaling on keep alive thread
        AutoResetEvent keepAliveEvent;
        AutoResetEvent keepAliveEventEnd;
        // last communication time in ticks
        int _lastCommTime;

        // event for PUBLISH message received
        public event MqttMsgPublishEventHandler MqttMsgPublishReceived;
        // event for published message
        public event MqttMsgPublishedEventHandler MqttMsgPublished;
        // event for subscribed topic
        public event MqttMsgSubscribedEventHandler MqttMsgSubscribed;
        // event for unsubscribed topic
        public event MqttMsgUnsubscribedEventHandler MqttMsgUnsubscribed;


        // event for peer/client disconnection
        public event ConnectionClosedEventHandler ConnectionClosed;

        // channel to communicate over the network
        IMqttNetworkChannel _channel;

        // inflight messages queue
        Queue<MqttMsgContext> _inflightQueue;
        // internal queue for received messages about inflight messages
        Queue<MqttMsgBase> _internalQueue;
        // internal queue for dispatching events
        Queue<InternalEvent> _eventQueue;
        // session
        MqttClientSession _session;

        // reference to avoid access to singleton via property
        MqttSettings _settings;

        // current message identifier generated
        ushort _messageIdCounter = 0;

        // connection is closing due to peer
        bool _isConnectionClosing;

        /// <summary>
        /// Connection status between client and broker
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Client identifier
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        /// Clean session flag
        /// </summary>
        public bool CleanSession { get; private set; }

        /// <summary>
        /// Will flag
        /// </summary>
        public bool WillFlag { get; private set; }

        /// <summary>
        /// Will QOS level
        /// </summary>
        public byte WillQosLevel { get; private set; }

        /// <summary>
        /// Will topic
        /// </summary>
        public string WillTopic { get; private set; }

        /// <summary>
        /// Will message
        /// </summary>
        public string WillMessage { get; private set; }

        /// <summary>
        /// MQTT protocol version
        /// </summary>
        public MqttProtocolVersion ProtocolVersion { get; set; }

        /// <summary>
        /// MQTT client settings
        /// </summary>
        public MqttSettings Settings
        {
            get { return this._settings; }
        }


        public MqttClient(IMqttNetworkChannel channel)
        {
            _channel = channel;
        }


        public void Init(string brokerHostName, int brokerPort, bool secure)
        {
            this._brokerHostName = brokerHostName;
            this._brokerPort = brokerPort;

            // reference to MQTT settings
            this._settings = MqttSettings.Instance;
            // set settings port based on secure connection or not
            if (!secure)
                this._settings.Port = this._brokerPort;
            else
                this._settings.SslPort = this._brokerPort;

            this._syncEndReceiving = new AutoResetEvent(false);
            this.keepAliveEvent = new AutoResetEvent(false);

            // queue for handling inflight messages (publishing and acknowledge)
            this._inflightWaitHandle = new AutoResetEvent(false);
            this._inflightQueue = new Queue<MqttMsgContext>();

            // queue for received message
            this._receiveEventWaitHandle = new AutoResetEvent(false);
            this._eventQueue = new Queue<InternalEvent>();
            this._internalQueue = new Queue<MqttMsgBase>();

            // session
            this._session = null;
        }


        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public Task<ConnAck> Connect(string clientId)
        {
            return this.Connect(clientId, null, null, false, MqttMsgConnect.QOS_LEVEL_AT_MOST_ONCE, false, null, null, true, MqttMsgConnect.KEEP_ALIVE_PERIOD_DEFAULT);
        }

        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public Task<ConnAck> Connect(string clientId, string username, string password)
        {
            return this.Connect(clientId, username, password, false, MqttMsgConnect.QOS_LEVEL_AT_MOST_ONCE, false, null, null, true, MqttMsgConnect.KEEP_ALIVE_PERIOD_DEFAULT);
        }

        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="cleanSession">Clean sessione flag</param>
        /// <param name="keepAlivePeriod">Keep alive period</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public Task<ConnAck> Connect(string clientId, string username, string password, bool cleanSession, ushort keepAlivePeriod)
        {
            return this.Connect(clientId, username, password, false, MqttMsgConnect.QOS_LEVEL_AT_MOST_ONCE, false, null, null, cleanSession, keepAlivePeriod);
        }

        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="willRetain">Will retain flag</param>
        /// <param name="willQosLevel">Will QOS level</param>
        /// <param name="willFlag">Will flag</param>
        /// <param name="willTopic">Will topic</param>
        /// <param name="willMessage">Will message</param>
        /// <param name="cleanSession">Clean sessione flag</param>
        /// <param name="keepAlivePeriod">Keep alive period</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public async Task<ConnAck> Connect(string clientId,
            string username,
            string password,
            bool willRetain,
            byte willQosLevel,
            bool willFlag,
            string willTopic,
            string willMessage,
            bool cleanSession,
            ushort keepAlivePeriod)
        {
            // create CONNECT message
            var connect = new MqttMsgConnect(clientId,
                username,
                password,
                willRetain,
                willQosLevel,
                willFlag,
                willTopic,
                willMessage,
                cleanSession,
                keepAlivePeriod,
                (byte)this.ProtocolVersion);

            await _channel.InitAsync(this._brokerHostName, _brokerPort, false);

            try
            {
                // connect to the broker
                this._channel.Connect();
            }
            catch (Exception ex)
            {
                throw new MqttConnectionException("Exception connecting to the broker", ex);
            }

            this._lastCommTime = 0;
            this._isRunning = true;
            this._isConnectionClosing = false;
            // start thread for receiving messages from broker
            Task.Run(() => { this.ReceiveThread(); });

            MqttMsgConnack connack = (MqttMsgConnack)this.SendReceive(connect);
            // if connection accepted, start keep alive timer and 
            if (connack.ReturnCode == MqttMsgConnack.CONN_ACCEPTED)
            {
                // set all client properties
                this.ClientId = clientId;
                this.CleanSession = cleanSession;
                this.WillFlag = willFlag;
                this.WillTopic = willTopic;
                this.WillMessage = willMessage;
                this.WillQosLevel = willQosLevel;

                this._keepAlivePeriod = keepAlivePeriod * 1000; // convert in ms

                // restore previous session
                this.RestoreSession();

                // keep alive period equals zero means turning off keep alive mechanism
                if (this._keepAlivePeriod != 0)
                {
                    // start thread for sending keep alive message to the broker
                    Task.Run(() => { this.KeepAliveThread(); });
                }

                // start thread for raising received message event from broker
                Task.Run(() => { this.DispatchEventThread(); });

                // start thread for handling inflight messages queue to broker asynchronously (publish and acknowledge)
                Task.Run(() => { this.ProcessInflightThread(); });

                this.IsConnected = true;
            }
            return (ConnAck)connack.ReturnCode;
        }

        /// <summary>
        /// Disconnect from broker
        /// </summary>
        public void Disconnect()
        {
            MqttMsgDisconnect disconnect = new MqttMsgDisconnect();
            this.Send(disconnect);

            // close client
            this.OnConnectionClosing();
        }

        /// <summary>
        /// Close client
        /// </summary>

        private void Close()
        {
            // stop receiving thread
            this._isRunning = false;

            // wait end receive event thread
            if (this._receiveEventWaitHandle != null)
                this._receiveEventWaitHandle.Set();

            // wait end process inflight thread
            if (this._inflightWaitHandle != null)
                this._inflightWaitHandle.Set();

            // unlock keep alive thread and wait
            this.keepAliveEvent.Set();

            if (this.keepAliveEventEnd != null)
                this.keepAliveEventEnd.WaitOne();

            // clear all queues
            this._inflightQueue.Clear();
            this._internalQueue.Clear();
            this._eventQueue.Clear();

            // close network channel
            this._channel.Close();

            this.IsConnected = false;
        }

        /// <summary>
        /// Execute ping to broker for keep alive
        /// </summary>
        /// <returns>PINGRESP message from broker</returns>
        private MqttMsgPingResp Ping()
        {
            MqttMsgPingReq pingreq = new MqttMsgPingReq();
            try
            {
                // broker must send PINGRESP within timeout equal to keep alive period
                return (MqttMsgPingResp)this.SendReceive(pingreq, this._keepAlivePeriod);
            }
            catch (Exception)
            {
                // client must close connection
                this.OnConnectionClosing();
                return null;
            }
        }

        /// <summary>
        /// Subscribe for message topics
        /// </summary>
        /// <param name="topics">List of topics to subscribe</param>
        /// <param name="qosLevels">QOS levels related to topics</param>
        /// <returns>Message Id related to SUBSCRIBE message</returns>
        public ushort Subscribe(string[] topics, byte[] qosLevels)
        {
            var subscribe = new MqttMsgSubscribe(topics, qosLevels) { MessageId = this.GetMessageId() };

            // enqueue subscribe request into the inflight queue
            this.EnqueueInflight(subscribe, MqttMsgFlow.ToPublish);

            return subscribe.MessageId;
        }

        /// <summary>
        /// Unsubscribe for message topics
        /// </summary>
        /// <param name="topics">List of topics to unsubscribe</param>
        /// <returns>Message Id in UNSUBACK message from broker</returns>
        public ushort Unsubscribe(string[] topics)
        {
            var unsubscribe = new MqttMsgUnsubscribe(topics) { MessageId = this.GetMessageId() };

            // enqueue unsubscribe request into the inflight queue
            this.EnqueueInflight(unsubscribe, MqttMsgFlow.ToPublish);

            return unsubscribe.MessageId;
        }

        /// <summary>
        /// Publish a message asynchronously (QoS Level 0 and not retained)
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data (payload)</param>
        /// <returns>Message Id related to PUBLISH message</returns>
        public ushort Publish(string topic, byte[] message)
        {
            return this.Publish(topic, message, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
        }

        /// <summary>
        /// Publish a message asynchronously
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data (payload)</param>
        /// <param name="qosLevel">QoS Level</param>
        /// <param name="retain">Retain flag</param>
        /// <returns>Message Id related to PUBLISH message</returns>
        public ushort Publish(string topic, byte[] message, byte qosLevel, bool retain)
        {
            var publish = new MqttMsgPublish(topic, message, false, qosLevel, retain) { MessageId = this.GetMessageId() };

            // enqueue message to publish into the inflight queue
            bool enqueue = this.EnqueueInflight(publish, MqttMsgFlow.ToPublish);

            // message enqueued
            if (enqueue)
                return publish.MessageId;
            // infligh queue full, message not enqueued
            else
                throw new MqttClientException(MqttClientErrorCode.InflightQueueFull);
        }

        /// <summary>
        /// Wrapper method for raising events
        /// </summary>
        /// <param name="internalEvent">Internal event</param>
        private void OnInternalEvent(InternalEvent internalEvent)
        {
            lock (this._eventQueue)
            {
                this._eventQueue.Enqueue(internalEvent);
            }

            this._receiveEventWaitHandle.Set();
        }

        /// <summary>
        /// Wrapper method for raising closing connection event
        /// </summary>
        private void OnConnectionClosing()
        {
            if (!_isConnectionClosing)
            {
                this._isConnectionClosing = true;
                this._receiveEventWaitHandle.Set();
            }
        }

        /// <summary>
        /// Wrapper method for raising PUBLISH message received event
        /// </summary>
        /// <param name="publish">PUBLISH message received</param>
        private void OnMqttMsgPublishReceived(MqttMsgPublish publish)
        {
            this.MqttMsgPublishReceived?.Invoke(this, new MqttMsgPublishEventArgs(publish.Topic, publish.Message, publish.DupFlag, publish.QosLevel, publish.Retain));
        }

        /// <summary>
        /// Wrapper method for raising published message event
        /// </summary>
        /// <param name="messageId">Message identifier for published message</param>
        /// <param name="isPublished">Publish flag</param>
        private void OnMqttMsgPublished(ushort messageId, bool isPublished)
        {
            this.MqttMsgPublished?.Invoke(this, new MqttMsgPublishedEventArgs(messageId, isPublished));
        }

        /// <summary>
        /// Wrapper method for raising subscribed topic event
        /// </summary>
        /// <param name="suback">SUBACK message received</param>
        private void OnMqttMsgSubscribed(MqttMsgSuback suback)
        {
            this.MqttMsgSubscribed?.Invoke(this, new MqttMsgSubscribedEventArgs(suback.MessageId, suback.GrantedQoSLevels));
        }

        /// <summary>
        /// Wrapper method for raising unsubscribed topic event
        /// </summary>
        /// <param name="messageId">Message identifier for unsubscribed topic</param>
        private void OnMqttMsgUnsubscribed(ushort messageId)
        {
            this.MqttMsgUnsubscribed?.Invoke(this, new MqttMsgUnsubscribedEventArgs(messageId));
        }

        /// <summary>
        /// Wrapper method for peer/client disconnection
        /// </summary>
        private void OnConnectionClosed()
        {
            this.ConnectionClosed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="msgBytes">Message bytes</param>
        private void Send(byte[] msgBytes)
        {
            try
            {
                // send message
                this._channel.Send(msgBytes);
            }
            catch (Exception e)
            {

                throw new MqttCommunicationException(e);
            }
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="msg">Message</param>
        private void Send(MqttMsgBase msg)
        {
            MqttUtility.Trace.WriteLine(TraceLevel.Frame, "SEND {0}", msg);
            this.Send(msg.GetBytes((byte)this.ProtocolVersion));
        }

        /// <summary>
        /// Send a message to the broker and wait answer
        /// </summary>
        /// <param name="msgBytes">Message bytes</param>
        /// <returns>MQTT message response</returns>
        private MqttMsgBase SendReceive(byte[] msgBytes)
        {
            return this.SendReceive(msgBytes, MqttSettings.MQTT_DEFAULT_TIMEOUT);
        }

        /// <summary>
        /// Send a message to the broker and wait answer
        /// </summary>
        /// <param name="msgBytes">Message bytes</param>
        /// <param name="timeout">Timeout for receiving answer</param>
        /// <returns>MQTT message response</returns>
        private MqttMsgBase SendReceive(byte[] msgBytes, int timeout)
        {
            // reset handle before sending
            this._syncEndReceiving.Reset();
            try
            {
                // send message
                this._channel.Send(msgBytes);

                // update last message sent ticks
                this._lastCommTime = Environment.TickCount;
            }
            catch (Exception e)
            {

                throw new MqttCommunicationException(e);
            }

            // wait for answer from broker
            if (this._syncEndReceiving.WaitOne(timeout))
            {
                // message received without exception
                if (this._exReceiving == null)
                    return this._msgReceived;
                // receiving thread catched exception
                else
                    throw this._exReceiving;
            }
            else
            {
                // throw timeout exception
                throw new MqttCommunicationException();
            }
        }

        /// <summary>
        /// Send a message to the broker and wait answer
        /// </summary>
        /// <param name="msg">Message</param>
        /// <returns>MQTT message response</returns>
        private MqttMsgBase SendReceive(MqttMsgBase msg)
        {
            return this.SendReceive(msg, MqttSettings.MQTT_DEFAULT_TIMEOUT);
        }

        /// <summary>
        /// Send a message to the broker and wait answer
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="timeout">Timeout for receiving answer</param>
        /// <returns>MQTT message response</returns>
        private MqttMsgBase SendReceive(MqttMsgBase msg, int timeout)
        {
#if TRACE
            MqttUtility.Trace.WriteLine(TraceLevel.Frame, "SEND {0}", msg);
#endif
            return this.SendReceive(msg.GetBytes((byte)this.ProtocolVersion), timeout);
        }

        /// <summary>
        /// Enqueue a message into the inflight queue
        /// </summary>
        /// <param name="msg">Message to enqueue</param>
        /// <param name="flow">Message flow (publish, acknowledge)</param>
        /// <returns>Message enqueued or not</returns>
        private bool EnqueueInflight(MqttMsgBase msg, MqttMsgFlow flow)
        {
            // enqueue is needed (or not)
            bool enqueue = true;

            // if it is a PUBLISH message with QoS Level 2
            if ((msg.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE) &&
                (msg.QosLevel == MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE))
            {
                lock (this._inflightQueue)
                {
                    // if it is a PUBLISH message already received (it is in the inflight queue), the publisher
                    // re-sent it because it didn't received the PUBREC. In this case, we have to re-send PUBREC

                    // NOTE : I need to find on message id and flow because the broker could be publish/received
                    //        to/from client and message id could be the same (one tracked by broker and the other by client)
                    MqttMsgContextFinder msgCtxFinder = new MqttMsgContextFinder(msg.MessageId, MqttMsgFlow.ToAcknowledge);
                    MqttMsgContext msgCtx = (MqttMsgContext)this._inflightQueue.Where(msgCtxFinder.Find).First();

                    // the PUBLISH message is alredy in the inflight queue, we don't need to re-enqueue but we need
                    // to change state to re-send PUBREC
                    if (msgCtx != null)
                    {
                        msgCtx.State = MqttMsgState.QueuedQos2;
                        msgCtx.Flow = MqttMsgFlow.ToAcknowledge;
                        enqueue = false;
                    }
                }
            }

            if (enqueue)
            {
                // set a default state
                MqttMsgState state = MqttMsgState.QueuedQos0;

                // based on QoS level, the messages flow between broker and client changes
                switch (msg.QosLevel)
                {
                    // QoS Level 0
                    case MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE:

                        state = MqttMsgState.QueuedQos0;
                        break;

                    // QoS Level 1
                    case MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE:

                        state = MqttMsgState.QueuedQos1;
                        break;

                    // QoS Level 2
                    case MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE:

                        state = MqttMsgState.QueuedQos2;
                        break;
                }

                // [v3.1.1] SUBSCRIBE and UNSUBSCRIBE aren't "officially" QOS = 1
                //          so QueuedQos1 state isn't valid for them
                if (msg.Type == MqttMsgBase.MQTT_MSG_SUBSCRIBE_TYPE)
                    state = MqttMsgState.SendSubscribe;
                else if (msg.Type == MqttMsgBase.MQTT_MSG_UNSUBSCRIBE_TYPE)
                    state = MqttMsgState.SendUnsubscribe;

                // queue message context
                MqttMsgContext msgContext = new MqttMsgContext()
                {
                    Message = msg,
                    State = state,
                    Flow = flow,
                    Attempt = 0
                };

                lock (this._inflightQueue)
                {
                    // check number of messages inside inflight queue 
                    enqueue = (this._inflightQueue.Count < this._settings.InflightQueueSize);

                    if (enqueue)
                    {
                        // enqueue message and unlock send thread
                        this._inflightQueue.Enqueue(msgContext);

#if TRACE
                        MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "enqueued {0}", msg);
#endif

                        // PUBLISH message
                        if (msg.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE)
                        {
                            // to publish and QoS level 1 or 2
                            if ((msgContext.Flow == MqttMsgFlow.ToPublish) &&
                                ((msg.QosLevel == MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE) ||
                                 (msg.QosLevel == MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE)))
                            {
                                if (this._session != null)
                                    this._session.InflightMessages.Add(msgContext.Key, msgContext);
                            }
                            // to acknowledge and QoS level 2
                            else if ((msgContext.Flow == MqttMsgFlow.ToAcknowledge) &&
                                     (msg.QosLevel == MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE))
                            {
                                if (this._session != null)
                                    this._session.InflightMessages.Add(msgContext.Key, msgContext);
                            }
                        }
                    }
                }
            }

            this._inflightWaitHandle.Set();

            return enqueue;
        }

        /// <summary>
        /// Enqueue a message into the internal queue
        /// </summary>
        /// <param name="msg">Message to enqueue</param>
        private void EnqueueInternal(MqttMsgBase msg)
        {
            // enqueue is needed (or not)
            bool enqueue = true;

            // if it is a PUBREL message (for QoS Level 2)
            if (msg.Type == MqttMsgBase.MQTT_MSG_PUBREL_TYPE)
            {
                lock (this._inflightQueue)
                {
                    // if it is a PUBREL but the corresponding PUBLISH isn't in the inflight queue,
                    // it means that we processed PUBLISH message and received PUBREL and we sent PUBCOMP
                    // but publisher didn't receive PUBCOMP so it re-sent PUBREL. We need only to re-send PUBCOMP.

                    // NOTE : I need to find on message id and flow because the broker could be publish/received
                    //        to/from client and message id could be the same (one tracked by broker and the other by client)
                    MqttMsgContextFinder msgCtxFinder = new MqttMsgContextFinder(msg.MessageId, MqttMsgFlow.ToAcknowledge);
                    MqttMsgContext msgCtx = (MqttMsgContext)this._inflightQueue.Where(msgCtxFinder.Find).First();

                    // the PUBLISH message isn't in the inflight queue, it was already processed so
                    // we need to re-send PUBCOMP only
                    if (msgCtx == null)
                    {
                        var pubcomp = new MqttMsgPubcomp() { MessageId = msg.MessageId };
                        this.Send(pubcomp);

                        enqueue = false;
                    }
                }
            }
            // if it is a PUBCOMP message (for QoS Level 2)
            else if (msg.Type == MqttMsgBase.MQTT_MSG_PUBCOMP_TYPE)
            {
                lock (this._inflightQueue)
                {
                    // if it is a PUBCOMP but the corresponding PUBLISH isn't in the inflight queue,
                    // it means that we sent PUBLISH message, sent PUBREL (after receiving PUBREC) and already received PUBCOMP
                    // but publisher didn't receive PUBREL so it re-sent PUBCOMP. We need only to ignore this PUBCOMP.

                    // NOTE : I need to find on message id and flow because the broker could be publish/received
                    //        to/from client and message id could be the same (one tracked by broker and the other by client)
                    MqttMsgContextFinder msgCtxFinder = new MqttMsgContextFinder(msg.MessageId, MqttMsgFlow.ToPublish);
                    MqttMsgContext msgCtx = (MqttMsgContext)this._inflightQueue.Where(msgCtxFinder.Find).First();

                    // the PUBLISH message isn't in the inflight queue, it was already sent so we need to ignore this PUBCOMP
                    if (msgCtx == null)
                    {
                        enqueue = false;
                    }
                }
            }
            // if it is a PUBREC message (for QoS Level 2)
            else if (msg.Type == MqttMsgBase.MQTT_MSG_PUBREC_TYPE)
            {
                lock (this._inflightQueue)
                {
                    // if it is a PUBREC but the corresponding PUBLISH isn't in the inflight queue,
                    // it means that we sent PUBLISH message more times (retries) but broker didn't send PUBREC in time
                    // the publish is failed and we need only to ignore this PUBREC.

                    // NOTE : I need to find on message id and flow because the broker could be publish/received
                    //        to/from client and message id could be the same (one tracked by broker and the other by client)
                    MqttMsgContextFinder msgCtxFinder = new MqttMsgContextFinder(msg.MessageId, MqttMsgFlow.ToPublish);
                    MqttMsgContext msgCtx = (MqttMsgContext)this._inflightQueue.Where(msgCtxFinder.Find).First();

                    // the PUBLISH message isn't in the inflight queue, it was already sent so we need to ignore this PUBREC
                    if (msgCtx == null)
                    {
                        enqueue = false;
                    }
                }
            }

            if (enqueue)
            {
                lock (this._internalQueue)
                {
                    this._internalQueue.Enqueue(msg);
                    MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "enqueued {0}", msg);
                    this._inflightWaitHandle.Set();
                }
            }
        }

        /// <summary>
        /// Thread for receiving messages
        /// </summary>
        private void ReceiveThread()
        {
            int readBytes = 0;
            byte[] fixedHeaderFirstByte = new byte[1];
            byte msgType;

            while (this._isRunning)
            {
                try
                {
                    readBytes = this._channel.Receive(fixedHeaderFirstByte);

                    if (readBytes > 0)
                    {
                        msgType = (byte)((fixedHeaderFirstByte[0] & MqttMsgBase.MSG_TYPE_MASK) >> MqttMsgBase.MSG_TYPE_OFFSET);

                        switch (msgType)
                        {
                            case MqttMsgBase.MQTT_MSG_CONNECT_TYPE: throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            case MqttMsgBase.MQTT_MSG_CONNACK_TYPE:
                                this._msgReceived = MqttMsgConnack.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", this._msgReceived);
                                this._syncEndReceiving.Set();
                                break;

                            case MqttMsgBase.MQTT_MSG_PINGREQ_TYPE: throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            case MqttMsgBase.MQTT_MSG_PINGRESP_TYPE:
                                this._msgReceived = MqttMsgPingResp.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", this._msgReceived);
                                this._syncEndReceiving.Set();
                                break;

                            case MqttMsgBase.MQTT_MSG_SUBSCRIBE_TYPE: throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            case MqttMsgBase.MQTT_MSG_SUBACK_TYPE:
                                var suback = MqttMsgSuback.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", suback);
                                this.EnqueueInternal(suback);
                                break;

                            case MqttMsgBase.MQTT_MSG_PUBLISH_TYPE:
                                var publish = MqttMsgPublish.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", publish);
                                this.EnqueueInflight(publish, MqttMsgFlow.ToAcknowledge);
                                break;

                            case MqttMsgBase.MQTT_MSG_PUBACK_TYPE:
                                MqttMsgPuback puback = MqttMsgPuback.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", puback);
                                this.EnqueueInternal(puback);
                                break;

                            case MqttMsgBase.MQTT_MSG_PUBREC_TYPE:
                                MqttMsgPubrec pubrec = MqttMsgPubrec.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", pubrec);
                                this.EnqueueInternal(pubrec);
                                break;

                            case MqttMsgBase.MQTT_MSG_PUBREL_TYPE:
                                MqttMsgPubrel pubrel = MqttMsgPubrel.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", pubrel);
                                this.EnqueueInternal(pubrel);
                                break;

                            case MqttMsgBase.MQTT_MSG_PUBCOMP_TYPE:
                                var pubcomp = MqttMsgPubcomp.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", pubcomp);
                                this.EnqueueInternal(pubcomp);
                                break;

                            case MqttMsgBase.MQTT_MSG_UNSUBSCRIBE_TYPE:
                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            case MqttMsgBase.MQTT_MSG_UNSUBACK_TYPE:
                                MqttMsgUnsuback unsuback = MqttMsgUnsuback.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this._channel);
                                MqttUtility.Trace.WriteLine(TraceLevel.Frame, "RECV {0}", unsuback);
                                this.EnqueueInternal(unsuback);
                                break;

                            case MqttMsgDisconnect.MQTT_MSG_DISCONNECT_TYPE:
                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            default:

                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);
                        }

                        this._exReceiving = null;
                    }
                    // zero bytes read, peer gracefully closed socket
                    else
                    {
                        // wake up thread that will notify connection is closing
                        this.OnConnectionClosing();
                    }
                }
                catch (Exception e)
                {

                    this._exReceiving = new MqttCommunicationException(e);

                    bool close = false;
                    if (e.GetType() == typeof(MqttClientException))
                    {
                        // [v3.1.1] scenarios the receiver MUST close the network connection
                        MqttClientException ex = e as MqttClientException;
                        close = ((ex.ErrorCode == MqttClientErrorCode.InvalidFlagBits) ||
                                (ex.ErrorCode == MqttClientErrorCode.InvalidProtocolName) ||
                                (ex.ErrorCode == MqttClientErrorCode.InvalidConnectFlags));
                    }

                    if (e.GetType() == typeof(SocketException))
                    {
                        close = true;
                    }

                    if (close)
                    {
                        Close();
                        // wake up thread that will notify connection is closing
                        this.OnConnectionClosing();
                    }
                }
            }
        }

        /// <summary>
        /// Thread for handling keep alive message
        /// </summary>
        private void KeepAliveThread()
        {
            int delta = 0;
            int wait = this._keepAlivePeriod;

            // create event to signal that current thread is end
            this.keepAliveEventEnd = new AutoResetEvent(false);

            while (this._isRunning)
            {
                // waiting...
                this.keepAliveEvent.WaitOne(wait);

                if (this._isRunning)
                {
                    delta = Environment.TickCount - this._lastCommTime;

                    // if timeout exceeded ...
                    if (delta >= this._keepAlivePeriod)
                    {
                        this.Ping();
                        wait = this._keepAlivePeriod;
                    }
                    else
                    {
                        // update waiting time
                        wait = this._keepAlivePeriod - delta;
                    }
                }
            }

            // signal thread end
            this.keepAliveEventEnd.Set();
        }

        /// <summary>
        /// Thread for raising event
        /// </summary>
        private void DispatchEventThread()
        {
            while (this._isRunning)
            {
                if ((this._eventQueue.Count == 0) && !this._isConnectionClosing)
                    this._receiveEventWaitHandle.WaitOne();

                // check if it is running or we are closing client
                if (this._isRunning)
                {
                    // get event from queue
                    InternalEvent internalEvent = null;
                    lock (this._eventQueue)
                    {
                        if (this._eventQueue.Count > 0)
                            internalEvent = (InternalEvent)this._eventQueue.Dequeue();
                    }

                    // it's an event with a message inside
                    if (internalEvent != null)
                    {
                        MqttMsgBase msg = ((MsgInternalEvent)internalEvent).Message;

                        if (msg != null)
                        {
                            switch (msg.Type)
                            {
                                case MqttMsgDisconnect.MQTT_MSG_DISCONNECT_TYPE:
                                case MqttMsgBase.MQTT_MSG_UNSUBSCRIBE_TYPE:
                                case MqttMsgBase.MQTT_MSG_SUBSCRIBE_TYPE:
                                case MqttMsgBase.MQTT_MSG_CONNECT_TYPE: throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                                case MqttMsgBase.MQTT_MSG_SUBACK_TYPE:
                                    this.OnMqttMsgSubscribed((MqttMsgSuback)msg);
                                    break;

                                // PUBLISH message received
                                case MqttMsgBase.MQTT_MSG_PUBLISH_TYPE:
                                    if (internalEvent.GetType() == typeof(MsgPublishedInternalEvent))
                                        this.OnMqttMsgPublished(msg.MessageId, false);
                                    else
                                        this.OnMqttMsgPublishReceived((MqttMsgPublish)msg);
                                    break;

                                case MqttMsgBase.MQTT_MSG_PUBACK_TYPE: this.OnMqttMsgPublished(msg.MessageId, true); break;
                                case MqttMsgBase.MQTT_MSG_PUBREL_TYPE: this.OnMqttMsgPublishReceived((MqttMsgPublish)msg); break;
                                case MqttMsgBase.MQTT_MSG_PUBCOMP_TYPE: this.OnMqttMsgPublished(msg.MessageId, true); break;
                                case MqttMsgBase.MQTT_MSG_UNSUBACK_TYPE: this.OnMqttMsgUnsubscribed(msg.MessageId); break;
                            }
                        }
                    }

                    // all events for received messages dispatched, check if there is closing connection
                    if ((this._eventQueue.Count == 0) && this._isConnectionClosing)
                    {
                        // client must close connection
                        this.Close();

                        // client raw disconnection
                        this.OnConnectionClosed();
                    }
                }
            }
        }

        /// <summary>
        /// Process inflight messages queue
        /// </summary>
        private void ProcessInflightThread()
        {
            MqttMsgContext msgContext = null;
            MqttMsgBase msgInflight = null;
            MqttMsgBase msgReceived = null;
            InternalEvent internalEvent = null;
            bool acknowledge = false;
            int timeout = Timeout.Infinite;
            int delta;
            bool msgReceivedProcessed = false;

            try
            {
                while (this._isRunning)
                {
                    // wait on message queueud to inflight
                    this._inflightWaitHandle.WaitOne(timeout);

                    // it could be unblocked because Close() method is joining
                    if (this._isRunning)
                    {
                        lock (this._inflightQueue)
                        {
                            // message received and peeked from internal queue is processed
                            // NOTE : it has the corresponding message in inflight queue based on messageId
                            //        (ex. a PUBREC for a PUBLISH, a SUBACK for a SUBSCRIBE, ...)
                            //        if it's orphan we need to remove from internal queue
                            msgReceivedProcessed = false;
                            acknowledge = false;
                            msgReceived = null;

                            // set timeout tu MaxValue instead of Infinte (-1) to perform
                            // compare with calcultad current msgTimeout
                            timeout = Int32.MaxValue;

                            // a message inflight could be re-enqueued but we have to
                            // analyze it only just one time for cycle
                            int count = this._inflightQueue.Count;
                            // process all inflight queued messages
                            while (count > 0)
                            {
                                count--;
                                acknowledge = false;
                                msgReceived = null;

                                // check to be sure that client isn't closing and all queues are now empty !
                                if (!this._isRunning)
                                    break;

                                // dequeue message context from queue
                                msgContext = (MqttMsgContext)this._inflightQueue.Dequeue();

                                // get inflight message
                                msgInflight = (MqttMsgBase)msgContext.Message;

                                switch (msgContext.State)
                                {
                                    case MqttMsgState.QueuedQos0:

                                        // QoS 0, PUBLISH message to send to broker, no state change, no acknowledge
                                        if (msgContext.Flow == MqttMsgFlow.ToPublish)
                                        {
                                            this.Send(msgInflight);
                                        }
                                        // QoS 0, no need acknowledge
                                        else if (msgContext.Flow == MqttMsgFlow.ToAcknowledge)
                                        {
                                            internalEvent = new MsgInternalEvent(msgInflight);
                                            // notify published message from broker (no need acknowledged)
                                            this.OnInternalEvent(internalEvent);
                                        }

                                        MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "processed {0}", msgInflight);
                                        break;

                                    case MqttMsgState.QueuedQos1:
                                    // [v3.1.1] SUBSCRIBE and UNSIBSCRIBE aren't "officially" QOS = 1
                                    case MqttMsgState.SendSubscribe:
                                    case MqttMsgState.SendUnsubscribe:

                                        // QoS 1, PUBLISH or SUBSCRIBE/UNSUBSCRIBE message to send to broker, state change to wait PUBACK or SUBACK/UNSUBACK
                                        if (msgContext.Flow == MqttMsgFlow.ToPublish)
                                        {
                                            msgContext.Timestamp = Environment.TickCount;
                                            msgContext.Attempt++;

                                            if (msgInflight.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE)
                                            {
                                                // PUBLISH message to send, wait for PUBACK
                                                msgContext.State = MqttMsgState.WaitForPuback;
                                                // retry ? set dup flag [v3.1.1] only for PUBLISH message
                                                if (msgContext.Attempt > 1)
                                                    msgInflight.DupFlag = true;
                                            }
                                            else if (msgInflight.Type == MqttMsgBase.MQTT_MSG_SUBSCRIBE_TYPE)
                                                // SUBSCRIBE message to send, wait for SUBACK
                                                msgContext.State = MqttMsgState.WaitForSuback;
                                            else if (msgInflight.Type == MqttMsgBase.MQTT_MSG_UNSUBSCRIBE_TYPE)
                                                // UNSUBSCRIBE message to send, wait for UNSUBACK
                                                msgContext.State = MqttMsgState.WaitForUnsuback;

                                            this.Send(msgInflight);

                                            // update timeout : minimum between delay (based on current message sent) or current timeout
                                            timeout = (this._settings.DelayOnRetry < timeout) ? this._settings.DelayOnRetry : timeout;

                                            // re-enqueue message (I have to re-analyze for receiving PUBACK, SUBACK or UNSUBACK)
                                            this._inflightQueue.Enqueue(msgContext);
                                        }
                                        // QoS 1, PUBLISH message received from broker to acknowledge, send PUBACK
                                        else if (msgContext.Flow == MqttMsgFlow.ToAcknowledge)
                                        {
                                            var puback = new MqttMsgPuback() { MessageId = msgInflight.MessageId };
                                            this.Send(puback);

                                            internalEvent = new MsgInternalEvent(msgInflight);
                                            // notify published message from broker and acknowledged
                                            this.OnInternalEvent(internalEvent);

                                            MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "processed {0}", msgInflight);
                                        }
                                        break;

                                    case MqttMsgState.QueuedQos2:

                                        // QoS 2, PUBLISH message to send to broker, state change to wait PUBREC
                                        if (msgContext.Flow == MqttMsgFlow.ToPublish)
                                        {
                                            msgContext.Timestamp = Environment.TickCount;
                                            msgContext.Attempt++;
                                            msgContext.State = MqttMsgState.WaitForPubrec;
                                            // retry ? set dup flag
                                            if (msgContext.Attempt > 1)
                                                msgInflight.DupFlag = true;

                                            this.Send(msgInflight);

                                            // update timeout : minimum between delay (based on current message sent) or current timeout
                                            timeout = (this._settings.DelayOnRetry < timeout) ? this._settings.DelayOnRetry : timeout;

                                            // re-enqueue message (I have to re-analyze for receiving PUBREC)
                                            this._inflightQueue.Enqueue(msgContext);
                                        }
                                        // QoS 2, PUBLISH message received from broker to acknowledge, send PUBREC, state change to wait PUBREL
                                        else if (msgContext.Flow == MqttMsgFlow.ToAcknowledge)
                                        {
                                            var pubrec = new MqttMsgPubrec() { MessageId = msgInflight.MessageId };
                                            msgContext.State = MqttMsgState.WaitForPubrel;

                                            this.Send(pubrec);

                                            // re-enqueue message (I have to re-analyze for receiving PUBREL)
                                            this._inflightQueue.Enqueue(msgContext);
                                        }
                                        break;

                                    case MqttMsgState.WaitForPuback:
                                    case MqttMsgState.WaitForSuback:
                                    case MqttMsgState.WaitForUnsuback:

                                        // QoS 1, waiting for PUBACK of a PUBLISH message sent or
                                        //        waiting for SUBACK of a SUBSCRIBE message sent or
                                        //        waiting for UNSUBACK of a UNSUBSCRIBE message sent or
                                        if (msgContext.Flow == MqttMsgFlow.ToPublish)
                                        {
                                            acknowledge = false;
                                            lock (this._internalQueue)
                                            {
                                                if (this._internalQueue.Count > 0)
                                                    msgReceived = (MqttMsgBase)this._internalQueue.Peek();
                                            }

                                            // it is a PUBACK message or a SUBACK/UNSUBACK message
                                            if (msgReceived != null)
                                            {
                                                // PUBACK message or SUBACK/UNSUBACK message for the current message
                                                if (((msgReceived.Type == MqttMsgBase.MQTT_MSG_PUBACK_TYPE) && (msgInflight.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE) && (msgReceived.MessageId == msgInflight.MessageId)) ||
                                                    ((msgReceived.Type == MqttMsgBase.MQTT_MSG_SUBACK_TYPE) && (msgInflight.Type == MqttMsgBase.MQTT_MSG_SUBSCRIBE_TYPE) && (msgReceived.MessageId == msgInflight.MessageId)) ||
                                                    ((msgReceived.Type == MqttMsgBase.MQTT_MSG_UNSUBACK_TYPE) && (msgInflight.Type == MqttMsgBase.MQTT_MSG_UNSUBSCRIBE_TYPE) && (msgReceived.MessageId == msgInflight.MessageId)))
                                                {
                                                    lock (this._internalQueue)
                                                    {
                                                        // received message processed
                                                        this._internalQueue.Dequeue();
                                                        acknowledge = true;
                                                        msgReceivedProcessed = true;
                                                        MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "dequeued {0}", msgReceived);
                                                    }

                                                    // if PUBACK received, confirm published with flag
                                                    if (msgReceived.Type == MqttMsgBase.MQTT_MSG_PUBACK_TYPE)
                                                        internalEvent = new MsgPublishedInternalEvent(msgReceived, true);
                                                    else
                                                        internalEvent = new MsgInternalEvent(msgReceived);

                                                    // notify received acknowledge from broker of a published message or subscribe/unsubscribe message
                                                    this.OnInternalEvent(internalEvent);

                                                    // PUBACK received for PUBLISH message with QoS Level 1, remove from session state
                                                    if ((msgInflight.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE) &&
                                                        (this._session != null) && (this._session.InflightMessages.ContainsKey(msgContext.Key)))
                                                    {
                                                        this._session.InflightMessages.Remove(msgContext.Key);
                                                    }

                                                    MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "processed {0}", msgInflight);
                                                }
                                            }

                                            // current message not acknowledged, no PUBACK or SUBACK/UNSUBACK or not equal messageid 
                                            if (!acknowledge)
                                            {
                                                delta = Environment.TickCount - msgContext.Timestamp;
                                                // check timeout for receiving PUBACK since PUBLISH was sent or
                                                // for receiving SUBACK since SUBSCRIBE was sent or
                                                // for receiving UNSUBACK since UNSUBSCRIBE was sent
                                                if (delta >= this._settings.DelayOnRetry)
                                                {
                                                    // max retry not reached, resend
                                                    if (msgContext.Attempt < this._settings.AttemptsOnRetry)
                                                    {
                                                        msgContext.State = MqttMsgState.QueuedQos1;

                                                        // re-enqueue message
                                                        this._inflightQueue.Enqueue(msgContext);

                                                        // update timeout (0 -> reanalyze queue immediately)
                                                        timeout = 0;
                                                    }
                                                    else
                                                    {
                                                        // if PUBACK for a PUBLISH message not received after retries, raise event for not published
                                                        if (msgInflight.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE)
                                                        {
                                                            // PUBACK not received in time, PUBLISH retries failed, need to remove from session inflight messages too
                                                            if ((this._session != null) && (this._session.InflightMessages.ContainsKey(msgContext.Key)))
                                                            {
                                                                this._session.InflightMessages.Remove(msgContext.Key);
                                                            }

                                                            internalEvent = new MsgPublishedInternalEvent(msgInflight, false);

                                                            // notify not received acknowledge from broker and message not published
                                                            this.OnInternalEvent(internalEvent);
                                                        }
                                                        // NOTE : not raise events for SUBACK or UNSUBACK not received
                                                        //        for the user no event raised means subscribe/unsubscribe failed
                                                    }
                                                }
                                                else
                                                {
                                                    // re-enqueue message (I have to re-analyze for receiving PUBACK, SUBACK or UNSUBACK)
                                                    this._inflightQueue.Enqueue(msgContext);

                                                    // update timeout
                                                    int msgTimeout = (this._settings.DelayOnRetry - delta);
                                                    timeout = (msgTimeout < timeout) ? msgTimeout : timeout;
                                                }
                                            }
                                        }
                                        break;

                                    case MqttMsgState.WaitForPubrec:

                                        // QoS 2, waiting for PUBREC of a PUBLISH message sent
                                        if (msgContext.Flow == MqttMsgFlow.ToPublish)
                                        {
                                            acknowledge = false;
                                            lock (this._internalQueue)
                                            {
                                                if (this._internalQueue.Count > 0)
                                                    msgReceived = (MqttMsgBase)this._internalQueue.Peek();
                                            }

                                            // it is a PUBREC message
                                            if ((msgReceived != null) && (msgReceived.Type == MqttMsgBase.MQTT_MSG_PUBREC_TYPE))
                                            {
                                                // PUBREC message for the current PUBLISH message, send PUBREL, wait for PUBCOMP
                                                if (msgReceived.MessageId == msgInflight.MessageId)
                                                {
                                                    lock (this._internalQueue)
                                                    {
                                                        // received message processed
                                                        this._internalQueue.Dequeue();
                                                        acknowledge = true;
                                                        msgReceivedProcessed = true;
                                                        MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "dequeued {0}", msgReceived);
                                                    }

                                                    var pubrel = new MqttMsgPubrel() { MessageId = msgInflight.MessageId };
                                                    msgContext.State = MqttMsgState.WaitForPubcomp;
                                                    msgContext.Timestamp = Environment.TickCount;
                                                    msgContext.Attempt = 1;

                                                    this.Send(pubrel);

                                                    // update timeout : minimum between delay (based on current message sent) or current timeout
                                                    timeout = (this._settings.DelayOnRetry < timeout) ? this._settings.DelayOnRetry : timeout;

                                                    // re-enqueue message
                                                    this._inflightQueue.Enqueue(msgContext);
                                                }
                                            }

                                            // current message not acknowledged
                                            if (!acknowledge)
                                            {
                                                delta = Environment.TickCount - msgContext.Timestamp;
                                                // check timeout for receiving PUBREC since PUBLISH was sent
                                                if (delta >= this._settings.DelayOnRetry)
                                                {
                                                    // max retry not reached, resend
                                                    if (msgContext.Attempt < this._settings.AttemptsOnRetry)
                                                    {
                                                        msgContext.State = MqttMsgState.QueuedQos2;

                                                        // re-enqueue message
                                                        this._inflightQueue.Enqueue(msgContext);

                                                        // update timeout (0 -> reanalyze queue immediately)
                                                        timeout = 0;
                                                    }
                                                    else
                                                    {
                                                        // PUBREC not received in time, PUBLISH retries failed, need to remove from session inflight messages too
                                                        if ((this._session != null) && (this._session.InflightMessages.ContainsKey(msgContext.Key)))
                                                        {
                                                            this._session.InflightMessages.Remove(msgContext.Key);
                                                        }

                                                        // if PUBREC for a PUBLISH message not received after retries, raise event for not published
                                                        internalEvent = new MsgPublishedInternalEvent(msgInflight, false);
                                                        // notify not received acknowledge from broker and message not published
                                                        this.OnInternalEvent(internalEvent);
                                                    }
                                                }
                                                else
                                                {
                                                    // re-enqueue message
                                                    this._inflightQueue.Enqueue(msgContext);

                                                    // update timeout
                                                    int msgTimeout = (this._settings.DelayOnRetry - delta);
                                                    timeout = (msgTimeout < timeout) ? msgTimeout : timeout;
                                                }
                                            }
                                        }
                                        break;

                                    case MqttMsgState.WaitForPubrel:

                                        // QoS 2, waiting for PUBREL of a PUBREC message sent
                                        if (msgContext.Flow == MqttMsgFlow.ToAcknowledge)
                                        {
                                            lock (this._internalQueue)
                                            {
                                                if (this._internalQueue.Count > 0)
                                                    msgReceived = (MqttMsgBase)this._internalQueue.Peek();
                                            }

                                            // it is a PUBREL message
                                            if ((msgReceived != null) && (msgReceived.Type == MqttMsgBase.MQTT_MSG_PUBREL_TYPE))
                                            {
                                                // PUBREL message for the current message, send PUBCOMP
                                                if (msgReceived.MessageId == msgInflight.MessageId)
                                                {
                                                    lock (this._internalQueue)
                                                    {
                                                        // received message processed
                                                        this._internalQueue.Dequeue();
                                                        msgReceivedProcessed = true;
                                                        MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "dequeued {0}", msgReceived);
                                                    }

                                                    var pubcomp = new MqttMsgPubcomp() { MessageId = msgInflight.MessageId };

                                                    this.Send(pubcomp);

                                                    internalEvent = new MsgInternalEvent(msgInflight);
                                                    // notify published message from broker and acknowledged
                                                    this.OnInternalEvent(internalEvent);

                                                    // PUBREL received (and PUBCOMP sent) for PUBLISH message with QoS Level 2, remove from session state
                                                    if ((msgInflight.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE) &&
                                                        (this._session != null) &&
                                                        (this._session.InflightMessages.ContainsKey(msgContext.Key)))
                                                    {
                                                        this._session.InflightMessages.Remove(msgContext.Key);
                                                    }

                                                    MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "processed {0}", msgInflight);
                                                }
                                                else
                                                {
                                                    // re-enqueue message
                                                    this._inflightQueue.Enqueue(msgContext);
                                                }
                                            }
                                            else
                                            {
                                                // re-enqueue message
                                                this._inflightQueue.Enqueue(msgContext);
                                            }
                                        }
                                        break;

                                    case MqttMsgState.WaitForPubcomp:

                                        // QoS 2, waiting for PUBCOMP of a PUBREL message sent
                                        if (msgContext.Flow == MqttMsgFlow.ToPublish)
                                        {
                                            acknowledge = false;
                                            lock (this._internalQueue)
                                            {
                                                if (this._internalQueue.Count > 0)
                                                    msgReceived = (MqttMsgBase)this._internalQueue.Peek();
                                            }

                                            // it is a PUBCOMP message
                                            if ((msgReceived != null) && (msgReceived.Type == MqttMsgBase.MQTT_MSG_PUBCOMP_TYPE))
                                            {
                                                // PUBCOMP message for the current message
                                                if (msgReceived.MessageId == msgInflight.MessageId)
                                                {
                                                    lock (this._internalQueue)
                                                    {
                                                        // received message processed
                                                        this._internalQueue.Dequeue();
                                                        acknowledge = true;
                                                        msgReceivedProcessed = true;
                                                        MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "dequeued {0}", msgReceived);
                                                    }

                                                    internalEvent = new MsgPublishedInternalEvent(msgReceived, true);
                                                    // notify received acknowledge from broker of a published message
                                                    this.OnInternalEvent(internalEvent);

                                                    // PUBCOMP received for PUBLISH message with QoS Level 2, remove from session state
                                                    if ((msgInflight.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE) &&
                                                        (this._session != null) &&
                                                        (this._session.InflightMessages.ContainsKey(msgContext.Key)))
                                                    {
                                                        this._session.InflightMessages.Remove(msgContext.Key);
                                                    }

                                                    MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "processed {0}", msgInflight);
                                                }
                                            }
                                            // it is a PUBREC message
                                            else if ((msgReceived != null) && (msgReceived.Type == MqttMsgBase.MQTT_MSG_PUBREC_TYPE))
                                            {
                                                // another PUBREC message for the current message due to a retransmitted PUBLISH
                                                // I'm in waiting for PUBCOMP, so I can discard this PUBREC
                                                if (msgReceived.MessageId == msgInflight.MessageId)
                                                {
                                                    lock (this._internalQueue)
                                                    {
                                                        // received message processed
                                                        this._internalQueue.Dequeue();
                                                        acknowledge = true;
                                                        msgReceivedProcessed = true;
                                                        MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "dequeued {0}", msgReceived);

                                                        // re-enqueue message
                                                        this._inflightQueue.Enqueue(msgContext);
                                                    }
                                                }
                                            }

                                            // current message not acknowledged
                                            if (!acknowledge)
                                            {
                                                delta = Environment.TickCount - msgContext.Timestamp;
                                                // check timeout for receiving PUBCOMP since PUBREL was sent
                                                if (delta >= this._settings.DelayOnRetry)
                                                {
                                                    // max retry not reached, resend
                                                    if (msgContext.Attempt < this._settings.AttemptsOnRetry)
                                                    {
                                                        msgContext.State = MqttMsgState.SendPubrel;

                                                        // re-enqueue message
                                                        this._inflightQueue.Enqueue(msgContext);

                                                        // update timeout (0 -> reanalyze queue immediately)
                                                        timeout = 0;
                                                    }
                                                    else
                                                    {
                                                        // PUBCOMP not received, PUBREL retries failed, need to remove from session inflight messages too
                                                        if ((this._session != null) &&
                                                            (this._session.InflightMessages.ContainsKey(msgContext.Key)))
                                                        {
                                                            this._session.InflightMessages.Remove(msgContext.Key);
                                                        }

                                                        // if PUBCOMP for a PUBLISH message not received after retries, raise event for not published
                                                        internalEvent = new MsgPublishedInternalEvent(msgInflight, false);
                                                        // notify not received acknowledge from broker and message not published
                                                        this.OnInternalEvent(internalEvent);
                                                    }
                                                }
                                                else
                                                {
                                                    // re-enqueue message
                                                    this._inflightQueue.Enqueue(msgContext);

                                                    // update timeout
                                                    int msgTimeout = (this._settings.DelayOnRetry - delta);
                                                    timeout = (msgTimeout < timeout) ? msgTimeout : timeout;
                                                }
                                            }
                                        }
                                        break;

                                    case MqttMsgState.SendPubrec:

                                        // TODO : impossible ? --> QueuedQos2 ToAcknowledge
                                        break;

                                    case MqttMsgState.SendPubrel:

                                        // QoS 2, PUBREL message to send to broker, state change to wait PUBCOMP
                                        if (msgContext.Flow == MqttMsgFlow.ToPublish)
                                        {
                                            var pubrel = new MqttMsgPubrel() { MessageId = msgInflight.MessageId };

                                            msgContext.State = MqttMsgState.WaitForPubcomp;
                                            msgContext.Timestamp = Environment.TickCount;
                                            msgContext.Attempt++;
                                            // retry ? set dup flag [v3.1.1] no needed
                                            if (this.ProtocolVersion == MqttProtocolVersion.Version_3_1)
                                            {
                                                if (msgContext.Attempt > 1)
                                                    pubrel.DupFlag = true;
                                            }

                                            this.Send(pubrel);

                                            // update timeout : minimum between delay (based on current message sent) or current timeout
                                            timeout = (this._settings.DelayOnRetry < timeout) ? this._settings.DelayOnRetry : timeout;

                                            // re-enqueue message
                                            this._inflightQueue.Enqueue(msgContext);
                                        }
                                        break;

                                    case MqttMsgState.SendPubcomp:
                                        // TODO : impossible ?
                                        break;
                                    case MqttMsgState.SendPuback:
                                        // TODO : impossible ? --> QueuedQos1 ToAcknowledge
                                        break;
                                    default:
                                        break;
                                }
                            }

                            // if calculated timeout is MaxValue, it means that must be Infinite (-1)
                            if (timeout == Int32.MaxValue)
                                timeout = Timeout.Infinite;

                            // if message received is orphan, no corresponding message in inflight queue
                            // based on messageId, we need to remove from the queue
                            if ((msgReceived != null) && !msgReceivedProcessed)
                            {
                                this._internalQueue.Dequeue();
                                MqttUtility.Trace.WriteLine(TraceLevel.Queuing, "dequeued {0} orphan", msgReceived);
                            }
                        }
                    }
                }
            }
            catch (MqttCommunicationException)
            {
                // possible exception on Send, I need to re-enqueue not sent message
                if (msgContext != null)
                    // re-enqueue message
                    this._inflightQueue.Enqueue(msgContext);

                // raise disconnection client event
                this.OnConnectionClosing();
            }
        }

        /// <summary>
        /// Restore session
        /// </summary>
        private void RestoreSession()
        {
            // if not clean session
            if (!this.CleanSession)
            {
                // there is a previous session
                if (this._session != null)
                {
                    lock (this._inflightQueue)
                    {
                        foreach (MqttMsgContext msgContext in this._session.InflightMessages.Values)
                        {
                            this._inflightQueue.Enqueue(msgContext);

                            // if it is a PUBLISH message to publish
                            if ((msgContext.Message.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE) &&
                                (msgContext.Flow == MqttMsgFlow.ToPublish))
                            {
                                // it's QoS 1 and we haven't received PUBACK
                                if ((msgContext.Message.QosLevel == MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE) &&
                                    (msgContext.State == MqttMsgState.WaitForPuback))
                                {
                                    // we haven't received PUBACK, we need to resend PUBLISH message
                                    msgContext.State = MqttMsgState.QueuedQos1;
                                }
                                // it's QoS 2
                                else if (msgContext.Message.QosLevel == MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE)
                                {
                                    // we haven't received PUBREC, we need to resend PUBLISH message
                                    if (msgContext.State == MqttMsgState.WaitForPubrec)
                                    {
                                        msgContext.State = MqttMsgState.QueuedQos2;
                                    }
                                    // we haven't received PUBCOMP, we need to resend PUBREL for it
                                    else if (msgContext.State == MqttMsgState.WaitForPubcomp)
                                    {
                                        msgContext.State = MqttMsgState.SendPubrel;
                                    }
                                }
                            }
                        }
                    }

                    // unlock process inflight queue
                    this._inflightWaitHandle.Set();
                }
                else
                {
                    // create new session
                    this._session = new MqttClientSession(this.ClientId);
                }
            }
            // clean any previous session
            else
            {
                if (this._session != null)
                    this._session.Clear();
            }
        }


        /// <summary>
        /// Generate the next message identifier
        /// </summary>
        /// <returns>Message identifier</returns>
        private ushort GetMessageId()
        {
            // if 0 or max UInt16, it becomes 1 (first valid messageId)
            this._messageIdCounter = ((this._messageIdCounter % UInt16.MaxValue) != 0) ? (ushort)(this._messageIdCounter + 1) : (ushort)1;
            return this._messageIdCounter;
        }

        /// <summary>
        /// Finder class for PUBLISH message inside a queue
        /// </summary>
        internal class MqttMsgContextFinder
        {
            // PUBLISH message id
            internal ushort MessageId { get; set; }
            // message flow into inflight queue
            internal MqttMsgFlow Flow { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="messageId">Message Id</param>
            /// <param name="flow">Message flow inside inflight queue</param>
            internal MqttMsgContextFinder(ushort messageId, MqttMsgFlow flow)
            {
                this.MessageId = messageId;
                this.Flow = flow;
            }

            internal bool Find(object item)
            {
                var msgCtx = (MqttMsgContext)item;
                return ((msgCtx.Message.Type == MqttMsgBase.MQTT_MSG_PUBLISH_TYPE) &&
                        (msgCtx.Message.MessageId == this.MessageId) &&
                        msgCtx.Flow == this.Flow);

            }
        }
    }

    /// <summary>
    /// MQTT protocol version
    /// </summary>
    public enum MqttProtocolVersion
    {
        Version_3_1 = MqttMsgConnect.PROTOCOL_VERSION_V3_1,
        Version_3_1_1 = MqttMsgConnect.PROTOCOL_VERSION_V3_1_1
    }
}