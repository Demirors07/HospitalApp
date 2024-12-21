using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HospitalApp.TagHelpers
{
    public class AnchTagHelper : TagHelper
    {
         public int FontWeight {get; set; } = 1000;

          public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.Attributes.SetAttribute("style", "text-decoration: none; font-size: 20px");
            output.Attributes.SetAttribute("class", "fw-bold fst-italic");
        }

    }
}