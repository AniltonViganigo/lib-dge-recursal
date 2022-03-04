using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace Recursal_DGE
{
    public class DGE_BB_Class : CodeActivity
    {
        [Category("Input")]
        [Description("Caminho do Arquivo do Extrato DGE BB")]
        [RequiredArgument]
        public InArgument<String> ArquivoTXT_DGEBB { get; set; }

        [Category("Input")]
        [Description("Caminho do Arquivo Com as Informações Extraídas Extrato DGE BB")]
        [RequiredArgument]
        public InArgument<String> ArquivoCSV_DGEBB { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var arquivoTXT_DGEBB = ArquivoTXT_DGEBB.Get(context);
                var arquivoCSV_DGEBB = ArquivoCSV_DGEBB.Get(context);

                if (File.Exists(arquivoTXT_DGEBB))
                {
                    using (StreamReader reader = new StreamReader(arquivoTXT_DGEBB))
                    {
                        var conteudo = reader.ReadToEnd();
                        if (conteudo != null)
                        {
                            LerArquivo(arquivoTXT_DGEBB, arquivoCSV_DGEBB);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }

        public void Execute(string arquivoTXT_DGEBB, string arquivoCSV_DGEBB)
        {
            try
            {
                if (File.Exists(arquivoTXT_DGEBB))
                {
                    using (StreamReader reader = new StreamReader(arquivoTXT_DGEBB))
                    {
                        var conteudo = reader.ReadToEnd();
                        if (conteudo.Contains("Banco do Brasil"))
                        {
                            LerArquivo(arquivoTXT_DGEBB, arquivoCSV_DGEBB);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }

        public void LerArquivo(string ArquivoTXT_DGEBB, string ArquivoCSV_DGEBB)
        {
            try
            {
                var ctaJudicial = "";
                var parcela = "";
                var processNumber = "";
                var reclamante = "";
                var reclamada = "";
                var saldoCapital = "";
                var saldoDataBase = "";
                var validaLinha = false;
                string line;
                var firstValue = true;

                if (File.Exists(ArquivoTXT_DGEBB))
                {
                    using (StreamReader reader = new StreamReader(ArquivoTXT_DGEBB))
                    {
                        File.WriteAllText(ArquivoCSV_DGEBB, "contaJudicial;parcela;processo;reclamante;saldoCapital;saldoDataBase\r\n", System.Text.Encoding.UTF8);
                        while ((line = reader.ReadLine()) != null)
                        {
                            //Extrai os valores dos Saldos: Capital e Data Base;
                            var regexGeral = ("\\s([0-9]+[\\,])?([0-9]+[\\.,])+([0-9]{2})+");
                            MatchCollection matches = Regex.Matches(line, regexGeral);
                            foreach (Match balanceValue in matches)
                            {
                                if (matches.Count == 2 & firstValue == true)
                                {
                                    saldoCapital = balanceValue.Value;
                                    firstValue = false;
                                    line = line.Replace(balanceValue.Value, "");
                                }
                                else
                                {
                                    saldoDataBase = balanceValue.Value;
                                    firstValue = true;
                                    line = line.Replace(balanceValue.Value, "");
                                }
                            }

                            if (line.Contains("Conv"))
                            {
                                var split = line.Split('-');
                                reclamada = split[0].Trim();
                                split = reclamada.Split(' ');
                                //reclamada = split[0] + " " + split[1];
                                reclamada = split[0];
                            }

                            if (line.Contains("Trabalhista"))
                            {
                                validaLinha = true;
                            }

                            if (validaLinha)
                            {
                                var regexCtaProcessual = "(\\s\\d{13}\\s\\d{4})";
                                Match match = Regex.Match(line, regexCtaProcessual, RegexOptions.ECMAScript);
                                if (match.Success)
                                {
                                    var splitLine = line.Replace("  "," ").Split(' ');
                                    ctaJudicial = splitLine[1];
                                    parcela = splitLine[2];
                                    processNumber = splitLine[3];

                                    line = line.Replace(ctaJudicial, "").Replace(parcela, "").Replace(processNumber, "").Replace("-", "").Replace(".","").TrimStart();

                                    //Formatação do Número do Processo
                                    if (processNumber.Length == 20 && !processNumber.Contains("-") && !processNumber.Contains(".") && !processNumber.Contains("/"))
                                    {
                                        processNumber = processNumber.Insert(7,"-").Insert(10,".").Insert(15,".").Insert(17,".").Insert(20,".");
                                    }

                                    //Verifica se a line contém dígitos no início
                                    var regexCleanLineA = ("^\\d+");
                                    match = Regex.Match(line, regexCleanLineA, RegexOptions.ECMAScript);
                                    if (match.Success)
                                    {
                                        line = line.Replace(match.Value, "");
                                    }
                                }                                                               

                                //Extrai as informações da Reclamada;
                                var regexReclamada = (reclamada + "(([^;]+)?)\\s");
                                match = Regex.Match(line, regexReclamada, RegexOptions.ECMAScript);
                                if (match.Success)
                                {
                                    reclamante = line.Replace(match.Value,"").Trim();
                                    if (reclamante != "")
                                    {
                                        File.AppendAllText(ArquivoCSV_DGEBB, "'" + ctaJudicial.ToString() + "'" + ";"
                                        + "'" + parcela.ToString() + "'" + ";" + "'" + processNumber.ToString() + "'" + ";"
                                        + reclamante.ToString() + ";" + saldoCapital.ToString() + ";"
                                        + saldoDataBase.ToString() + ";\r\n", System.Text.Encoding.UTF8);
                                    }
                                    
                                }
                                
                            }

                        }
                    }
                }


            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }
    }
}
