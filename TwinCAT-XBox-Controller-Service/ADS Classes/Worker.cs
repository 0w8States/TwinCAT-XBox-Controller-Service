﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TwinCAT_XBox_Controller_Service
{
    public class ServerWorker : BackgroundService
    {
        private readonly ILogger<ServerWorker> _logger;

        public ServerWorker(ILogger<ServerWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancel)
        {
            AdsXBoxServer server = new AdsXBoxServer(25733, "XBoxAdsServer", _logger);
            //CancellationTokenSource Canceller = new CancellationTokenSource();

            Task[] serverTasks = new Task[1];

            serverTasks[0] = server.ConnectServerAndWaitAsync(cancel);

            /*Task xbox_controller_task = new Task(() =>
            {
                while(!Canceller.IsCancellationRequested)
                {
                    // Debug Messages
                    //var threadId = Thread.CurrentThread.ManagedThreadId;
                    //Console.WriteLine("Task Loop Current Thread Id:" + threadId);
                    server.UpdateXboxValues();
                }
                if(Canceller.IsCancellationRequested)
                    _logger.LogInformation("Shutting Down XBox Monitoring Driver.");
            });

            xbox_controller_task.Start();*/

            Task shutdownTask = Task.Run(async () =>
            {   
                await Task.WhenAll(serverTasks);
                _logger.LogInformation("All AdsServers closed down.!");
            });

            Console.WriteLine("Press enter to shutdown servers ...");
            Console.ReadLine();

            
            server.Disconnect();

            await shutdownTask; // Wait for Shutdown of Servers
            //Canceller.Cancel();
        }
    }
}