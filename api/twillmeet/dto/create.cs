public class CreateTwilmeetDto
{
    public string? Thumbnail { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? Desc { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Category { get; set; }
    public string? Languange { get; set; }
    public string? Tags { get; set; }
    public float? Price { get; set; }
    public bool? IsPaid { get; set; }
    public bool? IsCertificate { get; set; }
    public bool? IsClass { get; set; }
    public string[]? Classes { get; set; }

}

public class CreateBuyTwilmeetDto
{

        public string? TypePayment { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? IdItem { get; set; }
        public float? Price { get; set; }

}