using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Jellyfin.Networking.Configuration;
using Jellyfin.Networking.Udp;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller;
using MediaBrowser.Model.ApiClient;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Networking.AutoDiscovery
{
    /// <summary>
    /// Defines the <see cref="ZeroConf" />.
    /// </summary>
    public class ZeroConf : IDisposable
    {
        /// <summary>
        /// The UDP port to use for zero configuration.
        /// </summary>
        private const int PortNumber = 7359;
        private readonly IServerApplicationHost _appHost;
        private readonly IConfigurationManager _configuration;
        private readonly INetworkManager _networkManager;
        private readonly ILogger _logger;
        private UdpProcess[]? _udpProcess;
        private bool _disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroConf"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance.</param>
        /// <param name="appHost">The <see cref="IServerApplicationHost"/> instance.</param>
        /// <param name="networkManager">The <see cref="INetworkManager"/> instance.</param>
        /// <param name="configurationManager">The <see cref="IConfigurationManager"/> instance.</param>
        public ZeroConf(
            ILogger logger,
            IServerApplicationHost appHost,
            INetworkManager networkManager,
            IConfigurationManager configurationManager)
        {
            _logger = logger;
            _appHost = appHost;
            _configuration = configurationManager;
            _networkManager = networkManager;
            _configuration.NamedConfigurationUpdated += ConfigurationUpdated;
            UpdateSettings(_configuration.GetNetworkConfiguration());
        }

        /// <summary>
        /// Releases managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases managed resources.
        /// </summary>
        /// <param name="disposing"><c>True</c> if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _configuration.NamedConfigurationUpdated -= ConfigurationUpdated;
                _logger.LogWarning("Shutting down auto discovery...");
                _udpProcess.DisposeClients();
            }

            _disposedValue = true;
        }

        /// <summary>
        /// Updates the zero configuration state.
        /// </summary>
        /// <param name="config">The <see cref="NetworkConfiguration"/>.</param>
        private void UpdateSettings(NetworkConfiguration config)
        {
            if (!config.AutoDiscovery)
            {
                if (_udpProcess == null)
                {
                    return;
                }

                _logger.LogWarning("Shutting down auto discovery...");
                _udpProcess.DisposeClients();
                return;
            }

            _udpProcess = UdpHelper.CreateMulticastClients(
                PortNumber,
                _networkManager.GetAllBindInterfaces(),
                ProcessMessage,
                _logger,
                null,
                config.AutoDiscoveryTracing);

            if (_udpProcess.Length == 0)
            {
                _logger.LogWarning("Unable to start listener on UDP port {PortNumber}", PortNumber);
            }
            else
            {
                _logger.LogDebug("Starting auto discovery...");
            }
        }

        /// <summary>
        /// Called when a named configuration is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="evt">The <see cref="ConfigurationUpdateEventArgs"/>.</param>
        private void ConfigurationUpdated(object? sender, ConfigurationUpdateEventArgs evt)
        {
            if (evt.Key.Equals("network", StringComparison.Ordinal))
            {
                UpdateSettings((NetworkConfiguration)evt.NewConfiguration);
            }
        }

        /// <summary>
        /// Processes any received messages.
        /// </summary>
        /// <param name="client">The <see cref="UdpProcess"/>.</param>
        /// <param name="data">The data received.</param>
        /// <param name="receivedFrom">The remote <see cref="IPEndPoint"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        private async Task ProcessMessage(UdpProcess client, string data, IPEndPoint receivedFrom)
        {
            if (data.Contains("who is JellyfinServer?", StringComparison.OrdinalIgnoreCase))
            {
                var response = new ServerDiscoveryInfo(
                    _appHost.GetSmartApiUrl(receivedFrom.Address),
                    _appHost.SystemId,
                    _appHost.FriendlyName);
                string reply = JsonSerializer.Serialize(response);

                try
                {
                    await client.SendUnicast(reply, receivedFrom).ConfigureAwait(false);
                }
                catch (SocketException ex)
                {
                    _logger.LogError(ex, "Error sending response to {Local}->{Remote}", client.LocalEndPoint.Address, receivedFrom.Address);
                }
            }
        }
    }
}