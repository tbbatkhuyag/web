using System.ComponentModel.DataAnnotations;

namespace edu.Models
{
    public class modelClass
    {
    }
    [MetadataType(typeof(GroupLesson.Metadata))]
    public partial class GroupLesson
    {
        private sealed class Metadata
        {
            public int Id { get; set; }
            [Display(Name = "Хичээлийн нэр")]
            [Required(ErrorMessage = "Хичээлийн нэр оруулна уу!")]

            public string Name { get; set; }
            [Display(Name = "Лого")]
            public string Logo { get; set; }
            public int? TeacherId { get; set; }
            public int? UniId { get; set; }
            public DateTime? Txtdatetime { get; set; }

        }
    }
    [MetadataType(typeof(Lesson.Metadata))]
    public partial class Lesson
    {
        private sealed class Metadata
        {
            public int Id { get; set; }
            [Required(ErrorMessage = "Сэдэвийн нэр ээ оруулна уу!")]
            [Display(Name = "Сэдэвийн нэр")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Үргэлжлэх хугацаа оруулна уу!")]
            [Display(Name = "Үргэлжлэх хугацаа нэр")]
            public int? ViewMinut { get; set; }
            [Display(Name = "Товч")]
            public string Txtmore { get; set; }
            [Display(Name = "Агуулга")]
            public string Txtcontent { get; set; }
            [Display(Name = "Дуу, сонсох")]
            public string Txtvoice { get; set; }
            [Display(Name = "Видео ")]
            public string Txtvideo { get; set; }
            [Display(Name = "Дасгал")]
            public string Txtwork { get; set; }
            public int? GId { get; set; }
            public int? TeacherId { get; set; }
            [Display(Name = "Огноо")]
            public DateTime? CreateDate { get; set; }
        }
    }

    [MetadataType(typeof(NewsCat.Metadata))]
    public partial class NewsCat
    {
        private sealed class Metadata
        {
            public int Id { get; set; }
            [Required(ErrorMessage = "Категори нэр ээ оруулна уу!")]
            [Display(Name = "Категори нэр ")]
            public string Txtname { get; set; }
            [Display(Name = "Дараалал ")]
            public int? TxtOrd { get; set; }
            [Display(Name = "Товч ")]
            public string Txtmore { get; set; }
            [Display(Name = "Агуулга ")]
            public string Txtcontent { get; set; }
            public int? SubId { get; set; }
            [Display(Name = "Огноо ")]

            public DateTime? TxtDate { get; set; }
            [Display(Name = "Төрөл ")]

            public int? Txttype { get; set; }
            [Display(Name = "Холбоос ")]

            public string Txtlink { get; set; }
            [Display(Name = "Нуух эсэх ")]

            public long? Vis { get; set; }

        }
    }

    [MetadataType(typeof(Newslist.Metadata))]
    public partial class Newslist
    {
        private sealed class Metadata
        {
            public int Id { get; set; }
            [Required(ErrorMessage = "Агуулга оруулна уу!")]
            [Display(Name = "Агуулга нэр ")]
            public string Txtname { get; set; }
            [Display(Name = "Товч агуулга ")]
            public string Txtmore { get; set; }
            [Display(Name = "Үндсэн агуулга ")]

            public string Txtcontent { get; set; }
            [Display(Name = "Огноо ")]

            public string Txtdate { get; set; }
            [Display(Name = "Цэс ")]

            public int? CatId { get; set; }
            [Display(Name = "Огноо ")]

            public DateTime? Txtdatetime { get; set; }


        }
    }
}
