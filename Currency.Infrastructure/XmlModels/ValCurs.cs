using System.Xml.Serialization;

namespace Currency.Infrastructure.XmlModels;

[XmlRoot("ValCurs")]
public class ValCurs
{
    [XmlAttribute("Date")]
    public string Date { get; set; }

    [XmlElement("Valute")]
    public List<Valute> Valutes { get; set; } = new();
}