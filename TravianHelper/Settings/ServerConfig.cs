using Microsoft.Practices.Prism.ViewModel;

namespace TravianHelper.Settings
{
    public class ServerConfig : DbEntity
    {
        private string _server;

        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                RaisePropertyChanged(() => Server);
            }
        }

        private string _domain = "kingdoms.com";

        public string Domain
        {
            get => _domain;
            set
            {
                _domain = value;
                RaisePropertyChanged(() => Domain);
            }
        }

        private string _region;

        public string Region// com, ru
        {
            get => _region;
            set
            {
                _region = value;
                RaisePropertyChanged(() => Region);
            }
        }

        private int _linkOffset;

        public int LinkOffset
        {
            get => _linkOffset;
            set
            {
                _linkOffset = value;
                RaisePropertyChanged(() => LinkOffset);
            }
        }

        public ServerConfig()
        {
            
        }

        public ServerConfig(string server, string domain, string region, int linkOffset)
        {
            Server     = server;
            Domain     = domain;
            Region     = region;
            LinkOffset = linkOffset;
        }

        public override string ToString()
        {
            return $"{Server}.{Domain}";
        }
    }
}
