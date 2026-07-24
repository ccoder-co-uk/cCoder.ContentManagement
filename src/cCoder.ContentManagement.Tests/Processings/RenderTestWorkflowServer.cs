// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace cCoder.ContentManagement.Tests.Processings;

internal static class RenderTestWorkflowServer
{
    internal static async Task<string> RunStringAsync(
        Func<string, string> action,
        string responseBody = "executed") =>
        (string)await RunAsync(
            action: workflowBaseUrl => action(arg: workflowBaseUrl),
            responseBody: responseBody);

    internal static async Task<RenderResult> RunRenderResultAsync(
        Func<string, RenderResult> action,
        string responseBody = "executed") =>
        (RenderResult)await RunAsync(
            action: workflowBaseUrl => action(arg: workflowBaseUrl),
            responseBody: responseBody);

    private static async Task<object> RunAsync(
        Func<string, object> action,
        string responseBody)
    {
        using TcpListener listener = new(localaddr: IPAddress.Loopback, port: 0);
        listener.Start();

        int port = ((IPEndPoint)listener.LocalEndpoint).Port;

        Task serverTask = Task.Run(function: async () =>
        {
            using TcpClient client = await listener.AcceptTcpClientAsync();
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream: stream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);

            int contentLength = 0;

            while (true)
            {
                string line = await reader.ReadLineAsync();

                if (string.IsNullOrEmpty(value: line))
                {
                    break;
                }

                if (line.StartsWith(value: "Content-Length:", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    int.TryParse(s: line["Content-Length:".Length..].Trim(), result: out contentLength);
                }
            }

            if (contentLength > 0)
            {
                char[] bodyBuffer = new char[contentLength];
                _ = await reader.ReadBlockAsync(buffer: bodyBuffer, index: 0, count: contentLength);
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(s: responseBody);

            string header =
                $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {responseBytes.Length}\r\nConnection: close\r\n\r\n";

            byte[] headerBytes = Encoding.ASCII.GetBytes(s: header);

            await stream.WriteAsync(buffer: headerBytes, offset: 0, count: headerBytes.Length);
            await stream.WriteAsync(buffer: responseBytes, offset: 0, count: responseBytes.Length);
            await stream.FlushAsync();
        });

        object result = action(arg: $"http://127.0.0.1:{port}/");
        await serverTask;
        return result;
    }
}