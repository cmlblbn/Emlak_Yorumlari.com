using System.Runtime.InteropServices;
using Emlak_Yorumlari_Entities.Models;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Emlak_Yorumlari_Entities.MyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Emlak_Yorumlari_Entities.MyContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
			
			// Adres tiplerini yüklüyoruz öncelikle
			context.Adress_Types.AddOrUpdate(new Adress_Type()
            {
                adress_type_id = 1,
                adress_type = "il",
                IsActive = true
            });

            context.Adress_Types.AddOrUpdate(new Adress_Type()
            {
                adress_type_id = 2,
                adress_type = "ilce",
                IsActive = true
            });

            context.Adress_Types.AddOrUpdate(new Adress_Type()
            {
                adress_type_id = 3,
                adress_type = "mahalle",
                IsActive = true
            });

            context.SaveChanges();

            // excel dosyası okunması
            Excel.Application application = new Excel.Application();

            Excel.Workbook workbook;

            workbook = application.Workbooks.Open(@"C:\Users\Cemal\Desktop\Emlak_Yorumlari.com\örnek excel\il_ilce_mahalle_ornek.xlsx");
            Excel.Worksheet worksheet = workbook.Sheets[2];
            Excel.Range xlRange = worksheet.UsedRange;


            Excel.Worksheet worksheet2 = workbook.Sheets[3];
            Excel.Range xlRange2 = worksheet2.UsedRange;


            Excel.Worksheet worksheet3 = workbook.Sheets[5];
            Excel.Range xlRange3 = worksheet3.UsedRange;

            

            for (int i = 2; i <= 82; i++)
            {

                // 1. kısım plaka, veri tabanında id kısmı il ekleme kısmı burası

                context.Adress_Descriptions.AddOrUpdate(new Adress_Description()
                {
                    adress_desc_id = int.Parse(xlRange.Cells[i, 1].Value2.ToString()),
                    adress_name = xlRange.Cells[i, 2].Value2.ToString(),
                    adress_type_id = 1,
                    parent_id = 0,
                    IsActive = true
                });



            }



            int detector = 0;
            for (int i = 2; i <= 959; i++)
            {

                //100. id üzerinde ilçeler başlıyor
                detector = int.Parse(xlRange2.Cells[i, 3].Value2.ToString()) + 81;

                // 2. kısım ilceler, il kodları ile birlikte veri tabanında üst referans vererek saklıyoruz
                context.Adress_Descriptions.AddOrUpdate(new Adress_Description()
                {
                    adress_desc_id = detector,
                    adress_name = xlRange2.Cells[i, 2].Value2.ToString(),
                    adress_type_id = 2,
                    parent_id = int.Parse(xlRange2.Cells[i, 1].Value2.ToString()),
                    IsActive = true

                });

            }





            int ustReferansMahalle = 82;
            int id = int.Parse(xlRange3.Cells[2, 2].Value2.ToString());
            for (int i = 2; i <= 50950; i++)
            {

                // üst referans güncellemeleri burada yapılıyor, sıralı id bilgisini kullanıyoruz
                if (id != int.Parse(xlRange3.Cells[i, 2].Value2.ToString()))
                {
                    id = int.Parse(xlRange3.Cells[i, 2].Value2.ToString());
                    ustReferansMahalle++;
                }
                string yer = (xlRange3.Cells[i, 4].Value2.ToString()).Replace("Mah.", "");
                yer = yer.Replace("Mah", "");
                yer = yer.Replace("Mahallesi", "");
                yer = yer.Replace("Köyü", "");

                // 3. kısım mahallelerin eklenmesi, işlem yaklaşık 1 saat sürüyor.
                context.Adress_Descriptions.AddOrUpdate(new Adress_Description()
                {
                    
                    adress_name = yer,
                    adress_type_id = 3,
                    parent_id = ustReferansMahalle,
                    IsActive = true

                });

                if (i % 1000 == 0)
                {
                    context.SaveChanges();
                }
            }

            context.SaveChanges();

            
			// okunan dosyanın kapatılması
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(worksheet);
            Marshal.ReleaseComObject(xlRange2);
            Marshal.ReleaseComObject(worksheet2);
            Marshal.ReleaseComObject(xlRange3);
            Marshal.ReleaseComObject(worksheet3);

            //close and release
            workbook.Close();
            Marshal.ReleaseComObject(workbook);

            //quit and release
            application.Quit();
            Marshal.ReleaseComObject(application);
        }
    }
}
