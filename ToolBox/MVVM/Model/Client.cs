namespace Praca_Inżynierska_v1.MVVM.Model
{
    public class Client
    {
        public int ClientId { get; set; }
        public string ClientType { get; set; } // "Osoba/Firma"
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName =>
            ClientType == "Firma"
                ? CompanyName
                : $"{FirstName} {LastName}".Trim();
        public string CompanyName { get; set; }
        public string NIP { get; set; }
        public string REGON { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }


        public Client Clone()
        {
            return new Client
            {
                ClientId = this.ClientId,
                ClientType = this.ClientType,
                FirstName = this.FirstName,
                LastName = this.LastName,
                CompanyName = this.CompanyName,
                NIP = this.NIP,
                REGON = this.REGON,
                Address = this.Address,
                Phone = this.Phone,
                Email = this.Email
            };
        }

        public void CopyFrom(Client source)
        {
            if (source == null) return;

            ClientType = source.ClientType;
            FirstName = source.FirstName;
            LastName = source.LastName;
            CompanyName = source.CompanyName;
            NIP = source.NIP;
            REGON = source.REGON;
            Address = source.Address;
            Phone = source.Phone;
            Email = source.Email;
        }
    }
}
