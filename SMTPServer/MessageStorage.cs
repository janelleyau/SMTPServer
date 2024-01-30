using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System.Buffers;

namespace SMTPServerTesting
{
    public class MessageStorage: MessageStore
    {
        readonly TextWriter _writer;

        public MessageStorage(TextWriter writer)
        {
            _writer = writer;
        }

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream();

            var position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out var memory))
            {
                stream.Write(memory.Span);
            }

            stream.Position = 0;

            var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);

            _writer.WriteLine("Subject={0}", message.Subject);
            _writer.WriteLine("Body={0}", message.Body);

            return SmtpResponse.Ok;
        }
    }
}
