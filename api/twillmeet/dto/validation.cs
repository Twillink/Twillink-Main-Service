using Microsoft.IdentityModel.Tokens;

public class ValidationTwilmeetDto
{
    public List<object> ValidateCreateInput(CreateTwilmeetDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Thumbnail))
        {
            errors.Add(new { Thumbnail = "Thumbnail is a required field." });
        }
        return errors;
    }

    public List<object> ValidateCreateBuyInput(CreateBuyTwilmeetDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.TypePayment))
        {
            errors.Add(new { TypePayment = "TypePayment is a required field." });
        }

        if (items == null || string.IsNullOrEmpty(items.IdItem))
        {
            errors.Add(new { IdItem = "IdItem is a required field." });
        }
        return errors;
    }
}