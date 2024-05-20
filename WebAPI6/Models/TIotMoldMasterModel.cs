namespace WebAPI6.Models
    {
    public class TIotMoldMasterModel
        {
        public int? Id { get; set; }
        public string? MoldNo { get; set; }
        public string? MoldName { get; set; }
        public string MoldSerial { get; set; } = null!;
        public string? MachineCd { get; set; }
        public decimal? MaintenanceQty { get; set; }
        public decimal? ScrapShot { get; set; }
        public decimal? ScrapQty { get; set; }
        }
    }
