﻿using mock.dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.infra
{
    public interface IPagamentoDao
    {
        void Salvar(Pagamento pagamento);
    }
}
