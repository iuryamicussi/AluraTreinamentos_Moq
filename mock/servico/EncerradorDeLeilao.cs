using mock.dominio;
using mock.infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.servico
{
    public class Carteiro : ICarteiro
    {
        public virtual void envia(Leilao leilao)
        {

        }
    }

    public class EncerradorDeLeilao
    {
        public int total { get; private set; }
        private IRepositorioDeLeiloes dao;
        private ICarteiro carteiro;

        public EncerradorDeLeilao(IRepositorioDeLeiloes dao,ICarteiro carteiro)
        {
            total = 0;
            this.dao = dao;
            this.carteiro = carteiro;
        }

        public virtual void encerra()
        {

            List<Leilao> todosLeiloesCorrentes = dao.correntes();
            Console.WriteLine(todosLeiloesCorrentes.Count);

            foreach (var l in todosLeiloesCorrentes)
            {

                if (comecouSemanaPassada(l))
                {
                    l.encerra();
                    total++;
                    dao.atualiza(l);
                    carteiro.envia(l);
                }
            }
        }


        private bool comecouSemanaPassada(Leilao leilao)
        {

            return diasEntre(leilao.data, DateTime.Now) >= 7;

        }

        private int diasEntre(DateTime inicio, DateTime fim)
        {
            int dias = (int)(fim - inicio).TotalDays;

            return dias;
        }

    }
}
