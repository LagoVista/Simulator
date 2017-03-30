using System.IO;
using System.Threading.Tasks;
using LagoVista.XPlat.Core.Services;

namespace LagoVista.XPlat.iOS.Services
{
    public class StorageService : StorageServiceBase
    {
        protected override Task<Stream> GetStreamAsync(string file)
        {
            return Task.FromResult(System.IO.File.Open(file, FileMode.OpenOrCreate) as Stream);
        }

        protected override Task PutAllTextAsync(string path, string contents)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();
            Task.Run(() =>
            {
                System.IO.File.WriteAllText(path, contents);
                tcs.SetResult(0);
            });
            return tcs.Task;
        }

        protected override Task PutStreamAsync(string path, Stream stream)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();
            Task.Run(() =>
            {
                using (var outputStream = System.IO.File.Open(path, FileMode.OpenOrCreate))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(outputStream);
                    tcs.SetResult(0);
                }
            });

            return tcs.Task;
        }

        protected override Task<string> ReadAllTextAsync(string path)
        {
            if (System.IO.File.Exists(path))
            {
                return Task.FromResult(System.IO.File.ReadAllText(path));
            }
            else
            {
                return Task.FromResult("");
            }
        }
    }
}