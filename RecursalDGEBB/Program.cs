using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursalDGEBB
{
    class Program
    {
        static void Main(string[] args)
        {
            var Recursal_DGEBB = new Recursal_DGE.DGE_BB_Class();
            Recursal_DGEBB.Execute(@"C:\Users\vigan\OneDrive\Área de Trabalho\Projetos ONE4\VML Recursal - Protótipo\NEW DGE BB\Arquivos Teste\SALDO_DGE BB_15.09.2021.txt",
                @"C:\Users\vigan\OneDrive\Área de Trabalho\Projetos ONE4\VML Recursal - Protótipo\NEW DGE BB\Arquivos Teste\SALDO_DGE BB_15.09.2021.csv");
            Console.WriteLine("Fim");
        }
    }
}
