using System;

namespace Pricefy.Challenge.Domain.Entities
{
    public class FileDetail
    {
        public string FileName { get; set; }

        public long TotalRegistries { get; set; }

        public long RegistriesImported { get; set; }

        public DateTime ImportDate { get; set; }
    }
}