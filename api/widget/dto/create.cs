public class CreateWidgetDto
{
    public string? Name { get; set; }

}

public class ChangeWidthDto
{
    public string? id { get; set; }
    public string? width { get; set; }

}

public class CreateLink
{
    public string? Title { get; set; }
    public string? Url { get; set; }
}

public class UpdatePos
{
    public string? Id { get; set; }
    public long? sequence { get; set; }

}

public class CreateText
{
    public string? Text { get; set; }
}

public class CreateMap
{
    public string? Caption { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }

}

public class CreateImage
{

    public string? Caption { get; set; }
    public string? Url { get; set; }
}

public class CreateProfile
{

    public string? Caption { get; set; }

    public string? UrlImage { get; set; }
}

public class CreateVideo
{

    public string? Caption { get; set; }

    public string? UrlVideo { get; set; }
}

public class CreateContent
{

    public string? Url { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }
}

public class CreateContact
{

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public class CreateCarausel
{

    public string? Caption { get; set; }

    public List<string>? AttachmentUrl {get; set;}
}

public class CreateWebinar
{

    public string? Title { get; set; }

    public string? UrlLink { get; set; }

    public string? Passcode { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Desc { get; set; }
}

public class CreateBanner
{

    public string? UrlImage { get; set; }
}

public class CreateSocial
{

    public string? UrlLink { get; set; }
    public int? Model { get; set; }
}