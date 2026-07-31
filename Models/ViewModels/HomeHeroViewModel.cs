using System.ComponentModel.DataAnnotations;
namespace OzelDersYonetim.Models.ViewModels;
public class HomeHeroViewModel
{
 [Required,StringLength(120),Display(Name="Üstteki küçük yazı")]public string Eyebrow{get;set;}=string.Empty;
 [Required,StringLength(180),Display(Name="Büyük ana başlık")]public string Title{get;set;}=string.Empty;
 [Required,StringLength(500),Display(Name="Başlık altındaki açıklama")]public string Description{get;set;}=string.Empty;
 [Required,StringLength(80),Display(Name="Formül kartı başlığı")]public string TopicLabel{get;set;}="Bugünün konusu";
 [Required,StringLength(160),Display(Name="Gösterilecek formül")]public string Formula{get;set;}="x² − 5x + 6 = 0";
 [Required,StringLength(160),Display(Name="Çözüm adımı")]public string Solution{get;set;}="(x − 2)(x − 3) = 0";
 [Required,StringLength(160),Display(Name="Sonuç")]public string Result{get;set;}="x = 2 veya x = 3";
 [Required,StringLength(40),Display(Name="Gelişim değeri")]public string ProgressValue{get;set;}="+24%";
 [Required,StringLength(80),Display(Name="Gelişim açıklaması")]public string ProgressLabel{get;set;}="Net gelişimi";
 [Required,StringLength(40),Display(Name="Birinci kısa değer")]public string StatOneValue{get;set;}="1:1";
 [Required,StringLength(80),Display(Name="Birinci kısa açıklama")]public string StatOneLabel{get;set;}="Birebir ilgi";
 [Required,StringLength(40),Display(Name="İkinci kısa değer")]public string StatTwoValue{get;set;}="%100";
 [Required,StringLength(80),Display(Name="İkinci kısa açıklama")]public string StatTwoLabel{get;set;}="Kişisel plan";
 [Required,StringLength(40),Display(Name="Üçüncü kısa değer")]public string StatThreeValue{get;set;}="5–8";
 [Required,StringLength(80),Display(Name="Üçüncü kısa açıklama")]public string StatThreeLabel{get;set;}="Sınıf düzeyi";
}
