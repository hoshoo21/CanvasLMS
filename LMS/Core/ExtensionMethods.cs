using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;

public static class ExtensionMethods
{
    public static T JsonToObject<T>(this string jsonObj)
    {
        JavaScriptSerializer _jsserializer = new JavaScriptSerializer();
        return _jsserializer.Deserialize<T>(jsonObj as string);
    }

    public static T FromJson<T>(this string obj)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return serializer.Deserialize<T>(obj as string);
    }

    public static T FromJson<T>(this object obj)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return serializer.Deserialize<T>(obj as string);
    }

    public static MvcHtmlString BasicCheckBoxFor<T>(this HtmlHelper<T> html,
                                                Expression<Func<T, bool>> expression,
                                                object htmlAttributes = null)
    {
        var result = html.CheckBoxFor(expression).ToString();
        const string pattern = @"<input name=""[^""]+"" type=""hidden"" value=""false"" />";
        var single = Regex.Replace(result, pattern, "");
        return MvcHtmlString.Create(single);
    }

    public static MvcHtmlString ClientIdFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
    {
        return MvcHtmlString.Create(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression)));
    }
    public static MvcHtmlString NameFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
    {
        return MvcHtmlString.Create(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression)));
    }

    public static List<SelectListItem> GetDropdownList<T>(this Enum e)
    {
        List<SelectListItem> lst = new List<SelectListItem>();
        if (e != null)
        {
            var values = Enum.GetValues(typeof(T));
            if (values != null && values.Length > 0)
            {
                foreach (var aValue in values)
                {
                    Type enumType = e.GetType();
                    var memberInfo = enumType.GetMember(aValue.ToString());
                    if (memberInfo != null && memberInfo.Length > 0)
                    {
                        string sDescription = aValue.ToString();
                        var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (attrs.Length > 0)
                        {
                            sDescription = ((DescriptionAttribute)attrs[0]).Description;
                        }
                        string sValue = Convert.ToString((int)aValue);
                        lst.Add(new SelectListItem() {
                            Text = sDescription
                            , Value = sValue
                        });
                    }
                }
            }
        }
        return lst;
    }

    public static string GetDescription<T>(this T enumerationValue) where T : struct
    {
        var type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
        }
        var memberInfo = type.GetMember(enumerationValue.ToString());
        if (memberInfo.Length > 0)
        {
            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs.Length > 0)
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        return enumerationValue.ToString();
    }
}