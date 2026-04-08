$dict = [ordered]@{
    "Search School, Online eductional centers, etc"="Сургууль, онлайн сургалтын төв зэргийг хайх";
    "Category"="Ангилал";
    "40 Instructors"="40 Багш";
    "23 Instructors"="23 Багш";
    "30 Instructors"="30 Багш";
    "45 Instructors"="45 Багш";
    "80 Instructors"="80 Багш";
    "Information About UI/UX Design Degree"="UI/UX Дизайны зэргийн талаарх мэдээлэл";
    "12+ Lesson"="12+ Хичээл";
    "11+ Lesson"="11+ Хичээл";
    "16+ Lesson"="16+ Хичээл";
    "10+ Lesson"="10+ Хичээл";
    "13+ Lesson"="13+ Хичээл";
    "7+ Lesson"="7+ Хичээл";
    "8+ Lesson"="8+ Хичээл";
    "9hr 30min"="9цаг 30минут";
    "6hr 30min"="6цаг 30минут";
    "12hr 30min"="12цаг 30минут";
    "8hr 30min"="8цаг 30минут";
    "10hr 30min"="10цаг 30минут";
    "7hr 30min"="7цаг 30минут";
    "11hr 30min"="11цаг 30минут";
    "BUY NOW"="ОДОО АВАХ";
    "Wordpress for Beginners - Master Wordpress Quickly"="Анхан шатныханд зориулсан Wordpress - Хурдан хугацаанд эзэмших";
    "FREE"="ҮНЭГҮЙ";
    "Sketch from A to Z (2022): Become an app designer"="Sketch А-аас Я хүртэл (2022): Апп дизайнер болох";
    "Learn Angular Fundamentals From beginning to advance lavel"="Angular-ийн үндсийг анхан шатнаас гүнзгийрүүлэн сурах";
    "Build Responsive Real World Websites with HTML5 and CSS3"="HTML5 болон CSS3 ашиглан бодит вебсайт бүтээх";
    "C# Developers Double Your Coding Speed with Visual Studio"="C# Хөгжүүлэгчид Visual Studio ашиглан код бичих хурдаа нэмэгдүүлэх";
    "Unlimited access to 360+ courses <br>and 1,600+ hands-on labs"="360+ курс болон 1,600+ практик лабораторид хязгааргүй нэвтрэх эрх";
    "For Instructor"="Багшид зориулсан";
    "Profile"="Профайл";
    "Login"="Нэвтрэх";
    "Register"="Бүртгүүлэх";
    "<p>Instructor</p>"="<p>Багш</p>";
    " Dashboard"=" Хянах самбар";
    "For Student"="Оюутанд зориулсан";
    "Student"="Оюутан";
    "News letter"="Мэдээллийн хуудас";
    "Enter your email address"="Имэйл хаягаа оруулна уу";
    "Web Developer"="Веб хөгжүүлэгч";
    "PHP Expert"="PHP Мэргэжилтэн";
    "UI Designer"="UI Дизайнер";
    "Java Developer"="Java Хөгжүүлэгч";
    "50 Students"="50 Оюутан";
    "40 Students"="40 Оюутан";
    "20 Students"="20 Оюутан";
    "30 Students"="30 Оюутан";
    "Learn JavaScript and Express to become a professional JavaScript"="Мэргэжлийн түвшинд хүрэхийн тулд JavaScript болон Express сурах";
    "Responsive Web Design Essentials HTML5 CSS3 and Bootstrap"="Респонсив веб дизайны үндэс HTML5 CSS3 болон Bootstrap";
    "The Complete App Design Course - UX, UI and Design Thinking"="Апп дизайны бүрэн курс - UX, UI болон Дизайн сэтгэлгээ";
    "Attract More Attention Sales And Profits"="Илүү их анхаарал, борлуулалт, ашиг татах";
    "11 Tips to Help You Get New Clients"="Шинэ үйлчлүүлэгчтэй болоход туслах 11 зөвлөгөө";
    "An Overworked Newspaper Editor"="Хэт их ажилласан сонины редактор";
    "A Solution Built for Teachers"="Багш нарт зориулагдсан шийдэл";
    "Marketing"="Маркетинг";
    "Sales Order"="Борлуулалтын захиалга";
    "Design"="Дизайн";
    "Seo"="Seo";
    "Analysis"="Анализ";
    "Development"="Хөгжүүлэлт";
    "Sales"="Борлуулалт";
    "Jun 15, 2022"="2022 оны 6 сарын 15";
    "May 20, 2022"="2022 оны 5 сарын 20";
    "May 25, 2022"="2022 оны 5 сарын 25";
    "Jul 15, 2022"="2022 оны 7 сарын 15";
    "Sep 25, 2022"="2022 оны 9 сарын 25";
    "May 15, 2022"="2022 оны 5 сарын 15";
    "Jun 20, 2022"="2022 оны 6 сарын 20";
    "April 15, 2022"="2022 оны 4 сарын 15"
}

$files = @("e:\www\iumc\edu\edu\Views\Shared\_Layouthome - Copy.cshtml", "e:\www\iumc\edu\edu\Views\Shared\_Layouthome.cshtml")

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw -Encoding UTF8
        foreach ($key in $dict.Keys) {
            $val = $dict[$key]
            $content = $content.Replace($key, $val)
        }
        Set-Content -Path $file -Value $content -Encoding UTF8
        Write-Host "Translated: $file"
    } else {
        Write-Host "File not found: $file"
    }
}
