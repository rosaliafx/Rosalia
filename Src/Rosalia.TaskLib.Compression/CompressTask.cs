namespace Rosalia.TaskLib.Compression
{
    using ICSharpCode.SharpZipLib.Zip;
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Standard;

    public class CompressTask<T> : ExtendedTask<T, CompressTaskInput, object>
    {
        protected override object Execute(CompressTaskInput input, TaskContext<T> context, ResultBuilder resultBuilder)
        {
            var destination = GetRequired(input, i => i.Destination);

            using (var zipStream = new ZipOutputStream(destination.WriteStream))
            {
                foreach (var fileToCompress in input.SourceFiles)
                {
                    var entry = new ZipEntry(fileToCompress.EntityPath);
                    entry.Size = fileToCompress.File.Length;

                    zipStream.PutNextEntry(entry);

                    using (var fileReadStream = fileToCompress.File.ReadStream)
                    {
                        fileReadStream.CopyTo(zipStream);
                    }

                    zipStream.CloseEntry();
                }

                zipStream.IsStreamOwner = true;
            }

            return null;
        }
    }
}