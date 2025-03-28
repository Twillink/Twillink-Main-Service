using Twillink.Shared.Models;

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
    public string? UrlThumbnail { get; set; }

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
    public string? UrlThumbnail { get; set; }
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

    public string? UrlThumbnail { get; set; }
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

public class CreateProfiles
{

    public string? FullName { get; set; }
    public string? Description { get; set; }
    public string? UrlBanner { get; set; }
    public string? UrlImageProfile { get; set; }
}

public class CreateSosmed
{

    public string? Key { get; set; }
    public string? Value { get; set; }
}

public class CreateCarausel
{

    public string? Caption { get; set; }

    public List<string>? attachmentIds {get; set;}
}

public class CreateWebinar
{

    public string? Title { get; set; }
    public string? UrlWebinar { get; set; }
    public string? UrlThumbnail { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? Passcode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

}

public class CreateSchedule
{
    // public DateTime? Date { get; set; }
    // public string? StartTime { get; set; }
    // public string? EndTime { get; set; }
    public string? Caption { get; set; }
    public List<ScheduleItem>? ScheduleItem { get; set; }


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