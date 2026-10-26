using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO PRA QUEM FOR LER (professor, Thais, eu do futuro):
    //
    // Essa é a tela principal do sistema Rota+.
    // Tem 6 abas, e cada uma faz o CRUD de uma coisa:
    //   - Veículo
    //   - Motorista
    //   - Rota
    //   - Preço do Combustível
    //   - Viagem (essa é a mais chata porque usa 3 FKs)
    //   - Consultar Viagem (com filtros)
    //
    // A ideia é sempre a mesma em todas as abas:
    //   1) Os campos de texto "globais" da classe
    //   2) Um DataGridView que mostra o que tem no banco
    //   3) Botões: Novo, Salvar, Editar, Excluir, Atualizar
    //
    // Padrão de nomes dos métodos:
    //   VeiSalvar, VeiEditar, VeiExcluir...  (Vei = Veículo)
    //   MotSalvar, MotEditar...              (Mot = Motorista)
    //   RotSalvar...                         (Rot = Rota)
    //   PreSalvar...                         (Pre = Preço)
    //   ViaSalvar...                         (Via = Viagem)
    // ============================================================

    public class FrmPrincipal : Form
    {
        private TabControl abas;

        // ============================================================
        // CAMPOS DA ABA VEÍCULO
        // Ficam globais aqui na classe pra eu poder acessar de qualquer
        // método daqui de dentro. Se fossem variáveis locais do método
        // CriarAbaVeiculo, os outros métodos não veriam elas.
        // ============================================================
        private TextBox txtVeiId;
        private TextBox txtVeiModelo;
        private TextBox txtVeiPlaca;
        private TextBox txtVeiConsumo;
        private TextBox txtVeiCarga;
        private DataGridView gridVei;

        // ============================================================
        // CAMPOS DA ABA MOTORISTA
        // ============================================================
        private TextBox txtMotId;
        private TextBox txtMotNome;
        private TextBox txtMotCnh;
        private TextBox txtMotTelefone;
        private TextBox txtMotCep;
        private DataGridView gridMot;

        // ============================================================
        // CAMPOS DA ABA ROTA
        // ============================================================
        private TextBox txtRotId;
        private TextBox txtRotOrigem;
        private TextBox txtRotDestino;
        private TextBox txtRotDistancia;
        private DataGridView gridRot;

        // ============================================================
        // CAMPOS DA ABA PREÇO COMBUSTÍVEL
        // ============================================================
        private TextBox txtPreId;
        private TextBox txtPreCombustivel;
        private TextBox txtPreValor;
        private DateTimePicker dtpPreData;
        private DataGridView gridPre;

        // ============================================================
        // CAMPOS DA ABA VIAGEM
        // Essa é a mais complexa porque usa ComboBox pra escolher
        // veículo, motorista e rota do banco.
        // ============================================================
        private TextBox txtViaId;
        private ComboBox cmbViaVeiculo;
        private ComboBox cmbViaMotorista;
        private ComboBox cmbViaRota;
        private DateTimePicker dtpViaSaida;
        private DateTimePicker dtpViaChegada;
        private CheckBox chkViaEmAndamento;
        private ComboBox cmbViaSituacao;
        private DataGridView gridVia;
        private TextBox txtViaCusto;

        // ============================================================
        // CAMPOS DA ABA CONSULTA / MONITORAMENTO
        // ============================================================
        private ComboBox cmbFiltroStatus;
        private DateTimePicker dtpFiltroDe;
        private DateTimePicker dtpFiltroAte;
        private DataGridView gridConsulta;

        // ============================================================
        // CAMPOS DO DASHBOARD (KPIs)
        // ============================================================
        private Label lblKpiViagens;
        private Label lblKpiEmAndamento;
        private Label lblKpiCusto;
        private Label lblKpiMotoristas;
        private Label lblKpiVeiculos;

        public FrmPrincipal()
        {
            this.Text = "Rota+ · Sistema de Gerenciamento Logístico";
            // ANOTAÇÃO: em vez de abrir maximizado (que ocupava a tela toda),
            // defini um tamanho fixo. Fica mais compacto e acompanha
            // o tamanho das abas. Se quiser redimensionar, o usuário pode
            // esticar a janela na mão (não travei o tamanho).
            this.WindowState = FormWindowState.Normal;
            this.ClientSize = new Size(1200, 700);  // tamanho inicial
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Estilo.MentaBemClaro;
            this.Font = new Font("Segoe UI", 10F);

            // ANOTAÇÃO: a ORDEM importa! As abas primeiro, depois o menu.
            // Se inverter, o TabControl fica escondido atrás do MenuStrip.
            MontarAbas();
            MontarMenu();
        }

        // ============================================================
        // MENU SUPERIOR
        // ============================================================
        private void MontarMenu()
        {
            MenuStrip menu = new MenuStrip();
            menu.BackColor = Estilo.Indigo;
            menu.ForeColor = Color.White;
            menu.Font = new Font("Segoe UI Semibold", 10F);
            menu.Padding = new Padding(10, 6, 10, 6);
            // Deixa o fundo do menu suspenso índigo
            menu.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());
            ToolStripMenuItem menuArquivo = new ToolStripMenuItem("Arquivo");
            menuArquivo.ForeColor = Color.White;

            // ============================================================
            // OPÇÃO 1: TROCAR DE USUÁRIO (LOGOUT)
            // ============================================================
            ToolStripMenuItem menuLogout = new ToolStripMenuItem("  Trocar de Usuário (Logout)");
            menuLogout.Click += (s, e) => FazerLogout();
            menuArquivo.DropDownItems.Add(menuLogout);

            // Linha separadora
            menuArquivo.DropDownItems.Add(new ToolStripSeparator());

            // ============================================================
            // OPÇÃO 2: FECHAR O SISTEMA DE VEZ
            // ============================================================
            ToolStripMenuItem menuSair = new ToolStripMenuItem("  Fechar Sistema");
            menuSair.Click += (s, e) =>
            {
                DialogResult r = MessageBox.Show(
                    "Deseja realmente fechar o sistema?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {
                    Log.Info("Sistema fechado pelo usuário.");
                    Application.Exit();
                }
            };
            menuArquivo.DropDownItems.Add(menuSair);

            // Adiciona o menu "Arquivo" na barra principal
            menu.Items.Add(menuArquivo);

            // ============================================================
            // NOME DO USUÁRIO (Canto direito)
            // ============================================================
            ToolStripLabel itemUsuario = new ToolStripLabel("👤  " + Session.Usuario + "  ·  " + Session.Perfil);
            itemUsuario.ForeColor = Color.White;
            itemUsuario.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            itemUsuario.Alignment = ToolStripItemAlignment.Right;
            menu.Items.Add(itemUsuario);

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
        } 

        // ============================================================
        // ABAS
        // ============================================================
        private void MontarAbas()
        {
            abas = new TabControl();
            abas.Dock = DockStyle.Fill;
            abas.Font = new Font("Segoe UI Semibold", 10.5F);
            abas.Padding = new Point(20, 8);
            abas.ItemSize = new Size(160, 38);
            abas.SizeMode = TabSizeMode.Fixed;

            // O Windows não deixa mudar a cor das abas direto.
            // Então desenhamos na mão com o evento DrawItem. Truque esperto.
            abas.DrawMode = TabDrawMode.OwnerDrawFixed;
            abas.DrawItem += Abas_DrawItem;

            abas.TabPages.Add(CriarAbaDashboard());
            abas.TabPages.Add(CriarAbaVeiculo());
            abas.TabPages.Add(CriarAbaMotorista());
            abas.TabPages.Add(CriarAbaRota());
            abas.TabPages.Add(CriarAbaPreco());
            abas.TabPages.Add(CriarAbaViagem());
            abas.TabPages.Add(CriarAbaConsulta());

            this.Controls.Add(abas);
        }

        private void Abas_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            TabPage aba = abas.TabPages[e.Index];
            Rectangle retangulo = abas.GetTabRect(e.Index);
            bool selecionada = (abas.SelectedIndex == e.Index);

            Color corFundo = selecionada ? Estilo.Menta : Estilo.LilasClaro;
            using (SolidBrush pincel = new SolidBrush(corFundo))
                g.FillRectangle(pincel, retangulo);

            Color corTexto = selecionada ? Estilo.IndigoEscuro : Estilo.TextoEscuro;
            using (Font fonte = new Font("Segoe UI Semibold", 10.5F))
            using (SolidBrush pincel = new SolidBrush(corTexto))
            {
                StringFormat formato = new StringFormat();
                formato.Alignment = StringAlignment.Center;
                formato.LineAlignment = StringAlignment.Center;
                g.DrawString(aba.Text, fonte, pincel, retangulo, formato);
            }
        }

        // ============================================================
        // HELPERS (funções que ajudam a montar tudo)
        // ============================================================

        private TabPage NovaAba(string titulo)
        {
            TabPage t = new TabPage(titulo);
            t.BackColor = Estilo.MentaBemClaro;
            t.Padding = new Padding(15);
            return t;
        }

        // Cria uma barra de botões com ícone.
        // ANOTAÇÃO: passei os handlers de clique como parâmetro
        // porque assim cada aba pode ter seus próprios métodos.
        // Se um handler for null, ele só mostra "em breve".
        private ToolStrip CriarToolbar(params (string texto, string icone, EventHandler click)[] botoes)
        {
            ToolStrip barra = new ToolStrip();
            barra.BackColor = Color.White;
            barra.GripStyle = ToolStripGripStyle.Hidden;
            barra.Padding = new Padding(10, 6, 10, 6);

            foreach (var b in botoes)
            {
                ToolStripButton botao = new ToolStripButton();
                botao.Text = "  " + b.texto;
                botao.Image = Icones.Desenhar(b.icone, Estilo.Indigo, 20);
                botao.ImageAlign = ContentAlignment.MiddleLeft;
                botao.TextAlign = ContentAlignment.MiddleRight;
                botao.Font = new Font("Segoe UI Semibold", 10F);
                botao.ForeColor = Estilo.TextoEscuro;
                botao.AutoSize = false;
                botao.Width = 105; // Deixei menor pra caberem 7 botões na tela
                botao.Height = 34;
                botao.Margin = new Padding(4, 0, 4, 0);

                if (b.click != null)
                    botao.Click += b.click;
                else
                    botao.Click += (s, e) => AvisoEmBreve();

                barra.Items.Add(botao);
            }
            return barra;
        }

        private Panel CriarPainelCampos(int altura)
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = altura;
            p.BackColor = Color.White;
            p.Padding = new Padding(20);
            return p;
        }

        private Label Lbl(string texto, int x, int y, Control pai)
        {
            Label l = new Label();
            l.Text = texto;
            l.Location = new Point(x, y);
            l.AutoSize = true;
            l.Font = new Font("Segoe UI Semibold", 10F);
            l.ForeColor = Estilo.TextoEscuro;
            l.BackColor = Color.Transparent;
            pai.Controls.Add(l);
            return l;
        }

        private TextBox Txt(int x, int y, int largura, Control pai, bool soLeitura = false)
        {
            TextBox t = new TextBox();
            t.Location = new Point(x, y);
            t.Width = largura;
            t.ReadOnly = soLeitura;
            t.BackColor = soLeitura ? Estilo.MentaBemClaro : Color.White;
            Estilo.TextBox(t);
            pai.Controls.Add(t);
            return t;
        }

        private DataGridView NovaGrid()
        {
            DataGridView g = new DataGridView();
            g.Dock = DockStyle.Fill;
            Estilo.Grid(g);
            return g;
        }

        private void AvisoEmBreve()
        {
            MessageBox.Show(
                "Essa parte ainda tá em construção 😅",
                "Aviso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }


        // ============================================================
        // ANOTAÇÃO:
        // Verifica se o usuário logado pode excluir.
        // Se não puder, mostra uma mensagem e devolve false.
        //
        // Assim a gente não repete essa checagem em cada método Excluir.
        // ============================================================
        private bool PodeExcluir()
        {
            if (!Session.IsAdmin)
            {
                Log.Info("Tentativa de exclusão negada para " + Session.Usuario);

                MessageBox.Show(
                    "Só administradores podem excluir registros.\n\n" +
                    "Seu perfil é: " + Session.Perfil,
                    "Acesso negado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private TabPage CriarAbaDashboard()
        {
            TabPage pagina = NovaAba("Dashboard");

            // ============================================================
            // 1. CARDS DE KPIs (em cima)
            // ============================================================
            Panel painelKpis = new Panel();
            painelKpis.Dock = DockStyle.Top;
            painelKpis.Height = 150;
            painelKpis.BackColor = Estilo.MentaBemClaro;
            painelKpis.Padding = new Padding(10);

            lblKpiViagens = CriarCard(painelKpis, "🚚 Viagens", "0", Estilo.Indigo, 20);
            lblKpiEmAndamento = CriarCard(painelKpis, "⏳ Em Andamento", "0", Estilo.Lilas, 250);
            lblKpiCusto = CriarCard(painelKpis, "💰 Custo Total", "R$ 0", Estilo.MentaEscuro, 480);
            lblKpiMotoristas = CriarCard(painelKpis, "👤 Motoristas", "0", Estilo.IndigoEscuro, 710);
            lblKpiVeiculos = CriarCard(painelKpis, "🚛 Veículos", "0", Estilo.LilasEscuro, 940);

            // ============================================================
            // 2. PAINEL DE BOAS-VINDAS (embaixo)
            // ============================================================
            Panel painelBemVindo = new Panel();
            painelBemVindo.Dock = DockStyle.Fill;
            painelBemVindo.BackColor = Color.White;
            painelBemVindo.Padding = new Padding(40);

            Label lblTituloBemVindo = new Label();
            lblTituloBemVindo.Text = "Bem-vindo(a) ao Rota+ 🚀";
            lblTituloBemVindo.Font = new Font("Segoe UI Black", 32F);
            lblTituloBemVindo.ForeColor = Estilo.IndigoEscuro;
            lblTituloBemVindo.AutoSize = true;
            lblTituloBemVindo.Location = new Point(40, 50);
            painelBemVindo.Controls.Add(lblTituloBemVindo);

            Label lblSubBemVindo = new Label();
            lblSubBemVindo.Text = "Seu sistema de gerenciamento logístico está pronto para uso.\n\n" +
                                  "Use as abas acima para cadastrar veículos, gerenciar motoristas, \n" +
                                  "criar rotas e acompanhar as viagens em tempo real.\n\n" +
                                  "Este painel resume os dados gerais do seu banco de dados.";
            lblSubBemVindo.Font = new Font("Segoe UI", 14F);
            lblSubBemVindo.ForeColor = Estilo.TextoEscuro;
            lblSubBemVindo.AutoSize = true;
            lblSubBemVindo.Location = new Point(45, 130);
            painelBemVindo.Controls.Add(lblSubBemVindo);

            // Adiciona na tela
            pagina.Controls.Add(painelBemVindo);
            pagina.Controls.Add(painelKpis);

            // Atualiza os dados quando a aba abre
            pagina.Enter += (s, e) => CarregarDashboard();

            return pagina;
        }
        // ============================================================
        // Busca os números no banco e atualiza os cards
        // ============================================================
        private void CarregarDashboard()
        {
            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    // --- KPIs ---
                    using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM viagem", conn))
                        lblKpiViagens.Text = Convert.ToInt64(cmd.ExecuteScalar()).ToString();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM viagem WHERE situacao = 'Em Andamento'", conn))
                        lblKpiEmAndamento.Text = Convert.ToInt64(cmd.ExecuteScalar()).ToString();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT IFNULL(SUM(custo_estimado), 0) FROM viagem", conn))
                    {
                        decimal custo = Convert.ToDecimal(cmd.ExecuteScalar());
                        lblKpiCusto.Text = "R$ " + custo.ToString("N2");
                    }

                    using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM motorista", conn))
                        lblKpiMotoristas.Text = Convert.ToInt64(cmd.ExecuteScalar()).ToString();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM veiculo", conn))
                        lblKpiVeiculos.Text = Convert.ToInt64(cmd.ExecuteScalar()).ToString();
                }
            }
            catch (MySqlException)
            {
                // Banco fora, ignora
            }
            catch (Exception ex)
            {
                Log.Erro("Erro ao carregar dashboard", ex);
            }
        }
        // ============================================================
        // Método auxiliar: cria um card colorido
        // ============================================================
        private Label CriarCard(Panel pai, string titulo, string valor, Color cor, int x)
        {
            Panel card = new Panel();
            card.Location = new Point(x, 20);
            card.Size = new Size(220, 110);
            card.BackColor = Color.White;
            card.Paint += (s, e) =>
            {
                // Borda lilás clara
                Rectangle r = card.ClientRectangle;
                r.Width -= 1;
                r.Height -= 1;
                using (Pen caneta = new Pen(Estilo.LilasClaro, 1))
                    e.Graphics.DrawRectangle(caneta, r);

                // Barra colorida do lado esquerdo
                using (SolidBrush pincel = new SolidBrush(cor))
                    e.Graphics.FillRectangle(pincel, 0, 0, 6, card.Height);
            };

            // Título do card
            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font("Segoe UI", 10F);
            lblTitulo.ForeColor = Color.Gray;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(20, 15);
            card.Controls.Add(lblTitulo);

            // Valor do card
            Label lblValor = new Label();
            lblValor.Text = valor;
            lblValor.Font = new Font("Segoe UI Semibold", 24F);
            lblValor.ForeColor = cor;
            lblValor.AutoSize = true;
            lblValor.Location = new Point(20, 45);
            card.Controls.Add(lblValor);

            pai.Controls.Add(card);

            return lblValor;
        }


        // ============================================================
        // ═══════════════════ ABA VEÍCULO ═══════════════════════════
        // ============================================================
        private TabPage CriarAbaVeiculo()
        {
          
            TabPage pagina = NovaAba("Veículo");

            ToolStrip barra = CriarToolbar(
                ("Novo", Icones.Novo, (s, e) => VeiNovo()),
                ("Salvar", Icones.Salvar, (s, e) => VeiSalvar()),
                ("Editar", Icones.Editar, (s, e) => VeiEditar()),
                ("Excluir", Icones.Excluir, (s, e) => VeiExcluir()),
                ("Atualizar", Icones.Atualizar, (s, e) => VeiCarregar()),
                ("CSV", Icones.Csv, (s, e) => Exportador.ExportarCSV(gridVei, "Veiculos")),
                ("Relatório", Icones.Pdf, (s, e) => Exportador.ExportarHTML(gridVei, "Veiculos"))
            );

           

            Panel painel = CriarPainelCampos(220);

            Lbl("ID:", 30, 25, painel);
            txtVeiId = Txt(90, 22, 90, painel, true);

            Lbl("Modelo:", 30, 65, painel);
            txtVeiModelo = Txt(140, 62, 320, painel);

            Lbl("Placa:", 30, 105, painel);
            txtVeiPlaca = Txt(140, 102, 150, painel);

            Lbl("Consumo médio (Km/L):", 30, 145, painel);
            txtVeiConsumo = Txt(240, 142, 120, painel);

            Lbl("Carga máxima (Kg):", 30, 185, painel);
            txtVeiCarga = Txt(240, 182, 120, painel);

            gridVei = NovaGrid();

            // Clicar numa linha carrega os dados nos campos.
            // Isso é bem "mão na roda", o usuário não precisa redigitar.
            gridVei.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                DataGridViewRow linha = gridVei.Rows[e.RowIndex];

                txtVeiId.Text = linha.Cells["veiculoID"].Value?.ToString();
                txtVeiModelo.Text = linha.Cells["modelo"].Value?.ToString();
                txtVeiPlaca.Text = linha.Cells["placa"].Value?.ToString();
                txtVeiConsumo.Text = linha.Cells["consumo_medio"].Value?.ToString();
                txtVeiCarga.Text = linha.Cells["carga_maxima"].Value?.ToString();
            };

            // ANOTAÇÃO: quando a aba for aberta, já carrega os dados.
            // Fica mais natural do que exigir que o usuário clique em Atualizar.
            pagina.Enter += (s, e) => VeiCarregar();

            pagina.Controls.Add(gridVei);
            pagina.Controls.Add(painel);
            pagina.Controls.Add(barra);
            return pagina;
        }

        private void VeiNovo()
        {
            txtVeiId.Clear();
            txtVeiModelo.Clear();
            txtVeiPlaca.Clear();
            txtVeiConsumo.Clear();
            txtVeiCarga.Clear();
            txtVeiModelo.Focus();
        }

        private void VeiSalvar()
        {
            // Validação básica — o banco tem NOT NULL, então melhor
            // avisar antes do que deixar o MySQL dar erro feio.
            if (string.IsNullOrWhiteSpace(txtVeiModelo.Text))
            {
                MessageBox.Show("Preencha o modelo do veículo.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtVeiPlaca.Text))
            {
                MessageBox.Show("Preencha a placa.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ANOTAÇÃO: Valida se a placa segue o padrão do Mercosul (ABC1D23) 
            // ou o padrão antigo (ABC1234). Evita cadastrar "banana" como placa.
            string placaFormatada = txtVeiPlaca.Text.Trim().ToUpper();
            System.Text.RegularExpressions.Regex regexPlaca =
                new System.Text.RegularExpressions.Regex(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$");

            if (!regexPlaca.IsMatch(placaFormatada))
            {
                MessageBox.Show("Placa inválida! Use o formato ABC1234 ou ABC1D23 (sem hífen).",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtVeiPlaca.Focus();
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    // ANOTAÇÃO: uso parâmetros (@modelo, @placa...) em vez de
                    // concatenar strings. Isso evita SQL Injection, que é
                    // quando alguém digita código malicioso no campo.
                    
                    string sql = @"INSERT INTO veiculo 
                                   (modelo, placa, consumo_medio, carga_maxima)
                                   VALUES (@modelo, @placa, @consumo, @carga)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@modelo", txtVeiModelo.Text.Trim());
                        cmd.Parameters.AddWithValue("@placa", txtVeiPlaca.Text.Trim().ToUpper());

                        // TryParse porque se o usuário deixar vazio,
                        // a gente salva 0 em vez de dar erro.
                        decimal consumo = 0;
                        decimal.TryParse(txtVeiConsumo.Text, out consumo);
                        cmd.Parameters.AddWithValue("@consumo", consumo);

                        decimal carga = 0;
                        decimal.TryParse(txtVeiCarga.Text, out carga);
                        cmd.Parameters.AddWithValue("@carga", carga);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Veículo cadastrado com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                VeiNovo();
                VeiCarregar();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                // Erro 1062 é aquele "duplicate entry" do MySQL,
                // que acontece quando tenta cadastrar placa repetida.
                MessageBox.Show("Já existe um veículo com essa placa!",
                    "Placa duplicada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro no banco: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VeiEditar()
        {
            if (string.IsNullOrEmpty(txtVeiId.Text))
            {
                MessageBox.Show("Selecione um veículo na lista primeiro.",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ANOTAÇÃO: Valida se a placa segue o padrão do Mercosul (ABC1D23) 
            // ou o padrão antigo (ABC1234). Evita cadastrar "banana" como placa.
            string placaFormatada = txtVeiPlaca.Text.Trim().ToUpper();
            System.Text.RegularExpressions.Regex regexPlaca =
                new System.Text.RegularExpressions.Regex(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$");

            if (!regexPlaca.IsMatch(placaFormatada))
            {
                MessageBox.Show("Placa inválida! Use o formato ABC1234 ou ABC1D23 (sem hífen).",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtVeiPlaca.Focus();
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"UPDATE veiculo 
                                   SET modelo = @modelo,
                                       placa = @placa,
                                       consumo_medio = @consumo,
                                       carga_maxima = @carga
                                   WHERE veiculoID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtVeiId.Text);
                        cmd.Parameters.AddWithValue("@modelo", txtVeiModelo.Text.Trim());
                        cmd.Parameters.AddWithValue("@placa", txtVeiPlaca.Text.Trim().ToUpper());

                        decimal consumo = 0;
                        decimal.TryParse(txtVeiConsumo.Text, out consumo);
                        cmd.Parameters.AddWithValue("@consumo", consumo);

                        decimal carga = 0;
                        decimal.TryParse(txtVeiCarga.Text, out carga);
                        cmd.Parameters.AddWithValue("@carga", carga);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Veículo atualizado!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                VeiNovo();
                VeiCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao editar: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VeiExcluir()

        {
            if (!PodeExcluir()) return;



            if (string.IsNullOrEmpty(txtVeiId.Text))
            {
                MessageBox.Show("Selecione um veículo primeiro.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pergunta antes pra evitar acidente
            DialogResult resp = MessageBox.Show(
                "Tem certeza que quer excluir este veículo?",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resp != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = "DELETE FROM veiculo WHERE veiculoID = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtVeiId.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Veículo excluído.", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                VeiNovo();
                VeiCarregar();
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                // Erro 1451 = Foreign Key Constraint (tem viagens vinculadas)
                MessageBox.Show(
                    "Não dá pra excluir este veículo porque ele já possui viagens cadastradas.",
                    "Ação Bloqueada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro no banco: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VeiCarregar()
        {
            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = "SELECT * FROM veiculo ORDER BY veiculoID DESC";

                    using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable tabela = new DataTable();
                        da.Fill(tabela);
                        gridVei.DataSource = tabela;
                    }
                }
            }
            catch (MySqlException)
            {
                // Se o banco não tiver rodando, deixa a grid vazia
                // em vez de encher a tela de erro toda hora.
                gridVei.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar veículos: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ═══════════════════ ABA MOTORISTA ═════════════════════════
        // ============================================================
        private TabPage CriarAbaMotorista()
        {
            TabPage pagina = NovaAba("Motorista");

            ToolStrip barra = CriarToolbar(
     ("Novo", Icones.Novo, (s, e) => MotNovo()),
     ("Salvar", Icones.Salvar, (s, e) => MotSalvar()),
     ("Editar", Icones.Editar, (s, e) => MotEditar()),
     ("Excluir", Icones.Excluir, (s, e) => MotExcluir()),
     ("Atualizar", Icones.Atualizar, (s, e) => MotCarregar()),
     ("CSV", Icones.Csv, (s, e) => Exportador.ExportarCSV(gridMot, "Motoristas")),
     ("Relatório", Icones.Pdf, (s, e) => Exportador.ExportarHTML(gridMot, "Motoristas"))
 );
            Panel painel = CriarPainelCampos(220);

            Lbl("ID:", 30, 25, painel);
            txtMotId = Txt(90, 22, 90, painel, true);

            Lbl("Nome:", 30, 65, painel);
            txtMotNome = Txt(140, 62, 340, painel);

            Lbl("CNH:", 30, 105, painel);
            txtMotCnh = Txt(140, 102, 180, painel);

            Lbl("Telefone:", 30, 145, painel);
            txtMotTelefone = Txt(140, 142, 180, painel);

            Lbl("CEP:", 30, 185, painel);
            txtMotCep = Txt(140, 182, 120, painel);

            gridMot = NovaGrid();
            gridMot.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                DataGridViewRow linha = gridMot.Rows[e.RowIndex];

                txtMotId.Text = linha.Cells["motoristaID"].Value?.ToString();
                txtMotNome.Text = linha.Cells["nome"].Value?.ToString();
                txtMotCnh.Text = linha.Cells["cnh"].Value?.ToString();
                txtMotTelefone.Text = linha.Cells["telefone"].Value?.ToString();
                txtMotCep.Text = linha.Cells["cep"].Value?.ToString();
            };

            pagina.Enter += (s, e) => MotCarregar();

            pagina.Controls.Add(gridMot);
            pagina.Controls.Add(painel);
            pagina.Controls.Add(barra);
            return pagina;
        }

        private void MotNovo()
        {
            txtMotId.Clear();
            txtMotNome.Clear();
            txtMotCnh.Clear();
            txtMotTelefone.Clear();
            txtMotCep.Clear();
            txtMotNome.Focus();
        }

        private void MotSalvar()
        {
            if (string.IsNullOrWhiteSpace(txtMotNome.Text))
            {
                MessageBox.Show("Preencha o nome do motorista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMotCnh.Text))
            {
                MessageBox.Show("Preencha a CNH.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"INSERT INTO motorista 
                                   (nome, cnh, telefone, cep)
                                   VALUES (@nome, @cnh, @telefone, @cep)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", txtMotNome.Text.Trim());
                        cmd.Parameters.AddWithValue("@cnh", txtMotCnh.Text.Trim());
                        cmd.Parameters.AddWithValue("@telefone", txtMotTelefone.Text.Trim());
                        cmd.Parameters.AddWithValue("@cep", txtMotCep.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Motorista cadastrado!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                MotNovo();
                MotCarregar();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Já existe um motorista com essa CNH!",
                    "CNH duplicada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MotEditar()
        {
            if (string.IsNullOrEmpty(txtMotId.Text))
            {
                MessageBox.Show("Selecione um motorista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"UPDATE motorista 
                                   SET nome = @nome,
                                       cnh = @cnh,
                                       telefone = @telefone,
                                       cep = @cep
                                   WHERE motoristaID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtMotId.Text);
                        cmd.Parameters.AddWithValue("@nome", txtMotNome.Text.Trim());
                        cmd.Parameters.AddWithValue("@cnh", txtMotCnh.Text.Trim());
                        cmd.Parameters.AddWithValue("@telefone", txtMotTelefone.Text.Trim());
                        cmd.Parameters.AddWithValue("@cep", txtMotCep.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Motorista atualizado!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                MotNovo();
                MotCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MotExcluir()
        {
            if (!PodeExcluir()) return;

            if (string.IsNullOrEmpty(txtMotId.Text))
            {
                MessageBox.Show("Selecione um motorista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resp = MessageBox.Show(
                "Excluir este motorista?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM motorista WHERE motoristaID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtMotId.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Motorista excluído.", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                MotNovo();
                MotCarregar();
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                // Erro 1451 = Foreign Key Constraint (tem viagens vinculadas)
                MessageBox.Show(
                    "Não dá pra excluir este motorista porque ele já possui viagens cadastradas.",
                    "Ação Bloqueada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro no banco: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MotCarregar()
        {
            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT * FROM motorista ORDER BY motoristaID DESC", conn))
                    {
                        DataTable tabela = new DataTable();
                        da.Fill(tabela);
                        gridMot.DataSource = tabela;
                    }
                }
            }
            catch (MySqlException) { gridMot.DataSource = null; }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ═══════════════════ ABA ROTA ══════════════════════════════
        // ============================================================
        private TabPage CriarAbaRota()
        {
            TabPage pagina = NovaAba("Rota");

            ToolStrip barra = CriarToolbar(
    ("Novo", Icones.Novo, (s, e) => RotNovo()),
    ("Salvar", Icones.Salvar, (s, e) => RotSalvar()),
    ("Editar", Icones.Editar, (s, e) => RotEditar()),
    ("Excluir", Icones.Excluir, (s, e) => RotExcluir()),
    ("Atualizar", Icones.Atualizar, (s, e) => RotCarregar()),
    ("CSV", Icones.Csv, (s, e) => Exportador.ExportarCSV(gridRot, "Rotas")),
    ("Relatório", Icones.Pdf, (s, e) => Exportador.ExportarHTML(gridRot, "Rotas"))
);

            Panel painel = CriarPainelCampos(200);

            Lbl("ID:", 30, 25, painel);
            txtRotId = Txt(90, 22, 90, painel, true);

            Lbl("Origem:", 30, 65, painel);
            txtRotOrigem = Txt(140, 62, 340, painel);

            Lbl("Destino:", 30, 105, painel);
            txtRotDestino = Txt(140, 102, 340, painel);

            Lbl("Distância (Km):", 30, 145, painel);
            txtRotDistancia = Txt(190, 142, 130, painel);

            // ANOTAÇÃO: Adicionando o botão que chama a API de rotas!
            Button btnCalcular = new Button();
            btnCalcular.Text = "Calcular";
            btnCalcular.Location = new Point(330, 141);
            btnCalcular.Width = 100;
            btnCalcular.Height = 28;
            Estilo.Botao(btnCalcular, Estilo.Lilas); // Usei lilás pra diferenciar dos outros
            btnCalcular.Click += async (s, e) => await CalcularRota();
            painel.Controls.Add(btnCalcular);

            gridRot = NovaGrid();
            gridRot.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                DataGridViewRow linha = gridRot.Rows[e.RowIndex];

                txtRotId.Text = linha.Cells["rotaID"].Value?.ToString();
                txtRotOrigem.Text = linha.Cells["origem"].Value?.ToString();
                txtRotDestino.Text = linha.Cells["destino"].Value?.ToString();
                txtRotDistancia.Text = linha.Cells["distancia"].Value?.ToString();
            };

            pagina.Enter += (s, e) => RotCarregar();

            pagina.Controls.Add(gridRot);
            pagina.Controls.Add(painel);
            pagina.Controls.Add(barra);
            return pagina;
        }

        private void RotNovo()
        {
            txtRotId.Clear();
            txtRotOrigem.Clear();
            txtRotDestino.Clear();
            txtRotDistancia.Clear();
            txtRotOrigem.Focus();
        }

        private void RotSalvar()
        {
            if (string.IsNullOrWhiteSpace(txtRotOrigem.Text) ||
                string.IsNullOrWhiteSpace(txtRotDestino.Text))
            {
                MessageBox.Show("Preencha Origem e Destino.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"INSERT INTO rota (origem, destino, distancia)
                                   VALUES (@origem, @destino, @distancia)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@origem", txtRotOrigem.Text.Trim());
                        cmd.Parameters.AddWithValue("@destino", txtRotDestino.Text.Trim());

                        decimal distancia = 0;
                        decimal.TryParse(txtRotDistancia.Text, out distancia);
                        cmd.Parameters.AddWithValue("@distancia", distancia);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Rota cadastrada!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RotNovo();
                RotCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ANOTAÇÃO:
        // Esse método chama a classe ApiRotas, que faz a chamada HTTP.
        // Ele é "async" (assíncrono) pra não travar a tela do sistema
        // enquanto a internet busca os dados. É uma boa prática!
        // ============================================================
        private async Task CalcularRota()
        {
            string origem = txtRotOrigem.Text.Trim();
            string destino = txtRotDestino.Text.Trim();

            if (string.IsNullOrWhiteSpace(origem) || string.IsNullOrWhiteSpace(destino))
            {
                MessageBox.Show("Preencha a Origem e o Destino antes de calcular.",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Muda o cursor pra "ampulheta" enquanto calcula
            this.Cursor = Cursors.WaitCursor;

            try
            {
                double distancia = await ApiRotas.CalcularDistanciaAsync(origem, destino);

                if (distancia > 0)
                {
                    // Preenche o campo automaticamente!
                    txtRotDistancia.Text = distancia.ToString("F2");
                    Log.Info($"Rota calculada via API: {origem} -> {destino} = {distancia} km");
                }
                else
                {
                    MessageBox.Show("Não consegui calcular a rota. Verifique se digitou o nome das cidades corretamente ou se a internet está funcionando.",
                        "Ops!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                // Volta o cursor ao normal, mesmo se der erro
                this.Cursor = Cursors.Default;
            }
        }
        private void RotEditar()
        {
            if (string.IsNullOrEmpty(txtRotId.Text))
            {
                MessageBox.Show("Selecione uma rota.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"UPDATE rota 
                                   SET origem = @origem,
                                       destino = @destino,
                                       distancia = @distancia
                                   WHERE rotaID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtRotId.Text);
                        cmd.Parameters.AddWithValue("@origem", txtRotOrigem.Text.Trim());
                        cmd.Parameters.AddWithValue("@destino", txtRotDestino.Text.Trim());

                        decimal distancia = 0;
                        decimal.TryParse(txtRotDistancia.Text, out distancia);
                        cmd.Parameters.AddWithValue("@distancia", distancia);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Rota atualizada!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RotNovo();
                RotCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RotExcluir()
        {
            if (!PodeExcluir()) return;

            if (string.IsNullOrEmpty(txtRotId.Text))
            {
                MessageBox.Show("Selecione uma rota.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resp = MessageBox.Show(
                "Excluir esta rota?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM rota WHERE rotaID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtRotId.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Rota excluída.", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RotNovo();
                RotCarregar();
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                // Erro 1451 = Foreign Key Constraint (tem viagens vinculadas)
                MessageBox.Show(
                    "Não dá pra excluir esta rota porque ela já está vinculada a uma viagem.",
                    "Ação Bloqueada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro no banco: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RotCarregar()
        {
            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT * FROM rota ORDER BY rotaID DESC", conn))
                    {
                        DataTable tabela = new DataTable();
                        da.Fill(tabela);
                        gridRot.DataSource = tabela;
                    }
                }
            }
            catch (MySqlException) { gridRot.DataSource = null; }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ═══════════════ ABA PREÇO COMBUSTÍVEL ═════════════════════
        // ============================================================
        private TabPage CriarAbaPreco()
        {
            TabPage pagina = NovaAba("Preço Combustível");

            ToolStrip barra = CriarToolbar(
    ("Novo", Icones.Novo, (s, e) => PreNovo()),
    ("Salvar", Icones.Salvar, (s, e) => PreSalvar()),
    ("Editar", Icones.Editar, (s, e) => PreEditar()),
    ("Excluir", Icones.Excluir, (s, e) => PreExcluir()),
    ("Atualizar", Icones.Atualizar, (s, e) => PreCarregar()),
    ("CSV", Icones.Csv, (s, e) => Exportador.ExportarCSV(gridPre, "Precos")),
    ("Relatório", Icones.Pdf, (s, e) => Exportador.ExportarHTML(gridPre, "Precos"))
);

            Panel painel = CriarPainelCampos(200);

            Lbl("ID:", 30, 25, painel);
            txtPreId = Txt(90, 22, 90, painel, true);

            Lbl("Combustível:", 30, 65, painel);
            txtPreCombustivel = Txt(190, 62, 290, painel);

            Lbl("Preço (R$):", 30, 105, painel);
            txtPreValor = Txt(190, 102, 130, painel);

            Lbl("Data da consulta:", 30, 145, painel);
            dtpPreData = new DateTimePicker();
            dtpPreData.Location = new Point(190, 142);
            dtpPreData.Width = 180;
            dtpPreData.Format = DateTimePickerFormat.Short;
            dtpPreData.Font = new Font("Segoe UI", 11F);
            painel.Controls.Add(dtpPreData);

            gridPre = NovaGrid();
            gridPre.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                DataGridViewRow linha = gridPre.Rows[e.RowIndex];

                txtPreId.Text = linha.Cells["precoID"].Value?.ToString();
                txtPreCombustivel.Text = linha.Cells["combustivel"].Value?.ToString();
                txtPreValor.Text = linha.Cells["preco"].Value?.ToString();

                // Tenta converter a data que veio do banco
                DateTime data;
                if (DateTime.TryParse(linha.Cells["data_consulta"].Value?.ToString(), out data))
                    dtpPreData.Value = data;
            };

            pagina.Enter += (s, e) => PreCarregar();

            pagina.Controls.Add(gridPre);
            pagina.Controls.Add(painel);
            pagina.Controls.Add(barra);
            return pagina;
        }

        private void PreNovo()
        {
            txtPreId.Clear();
            txtPreCombustivel.Clear();
            txtPreValor.Clear();
            dtpPreData.Value = DateTime.Now;
            txtPreCombustivel.Focus();
        }

        private void PreSalvar()
        {
            if (string.IsNullOrWhiteSpace(txtPreCombustivel.Text) ||
                string.IsNullOrWhiteSpace(txtPreValor.Text))
            {
                MessageBox.Show("Preencha Combustível e Preço.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"INSERT INTO preco_combustivel 
                                   (combustivel, preco, data_consulta)
                                   VALUES (@comb, @preco, @data)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@comb", txtPreCombustivel.Text.Trim());

                        decimal preco = 0;
                        decimal.TryParse(txtPreValor.Text, out preco);
                        cmd.Parameters.AddWithValue("@preco", preco);

                        cmd.Parameters.AddWithValue("@data", dtpPreData.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Preço cadastrado!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                PreNovo();
                PreCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PreEditar()
        {
            if (string.IsNullOrEmpty(txtPreId.Text))
            {
                MessageBox.Show("Selecione um registro.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"UPDATE preco_combustivel 
                                   SET combustivel = @comb,
                                       preco = @preco,
                                       data_consulta = @data
                                   WHERE precoID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtPreId.Text);
                        cmd.Parameters.AddWithValue("@comb", txtPreCombustivel.Text.Trim());

                        decimal preco = 0;
                        decimal.TryParse(txtPreValor.Text, out preco);
                        cmd.Parameters.AddWithValue("@preco", preco);

                        cmd.Parameters.AddWithValue("@data", dtpPreData.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Preço atualizado!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                PreNovo();
                PreCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PreExcluir()
        {

            if (!PodeExcluir()) return;


            if (string.IsNullOrEmpty(txtPreId.Text))
            {
                MessageBox.Show("Selecione um registro.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resp = MessageBox.Show(
                "Excluir este registro?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM preco_combustivel WHERE precoID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtPreId.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Registro excluído.", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                PreNovo();
                PreCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PreCarregar()
        {
            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT * FROM preco_combustivel ORDER BY data_consulta DESC", conn))
                    {
                        DataTable tabela = new DataTable();
                        da.Fill(tabela);
                        gridPre.DataSource = tabela;
                    }
                }
            }
            catch (MySqlException) { gridPre.DataSource = null; }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ═══════════════════ ABA VIAGEM ════════════════════════════
        // ============================================================
        // ANOTAÇÃO: essa é a aba mais complexa porque a viagem
        // se relaciona com 3 tabelas (veículo, motorista e rota).
        // Os ComboBox são preenchidos com os dados dessas tabelas,
        // e quando a gente salva, grava só os IDs deles.
        // ============================================================
        private TabPage CriarAbaViagem()
        {
            TabPage pagina = NovaAba("Viagem");

            ToolStrip barra = CriarToolbar(
    ("Novo", Icones.Novo, (s, e) => ViaNovo()),
    ("Salvar", Icones.Salvar, (s, e) => ViaSalvar()),
    ("Editar", Icones.Editar, (s, e) => ViaEditar()),
    ("Excluir", Icones.Excluir, (s, e) => ViaExcluir()),
    ("Atualizar", Icones.Atualizar, (s, e) => { ViaCarregar(); ViaCarregarCombos(); }
            ),
    ("CSV", Icones.Csv, (s, e) => Exportador.ExportarCSV(gridVia, "Viagens")),
    ("Relatório", Icones.Pdf, (s, e) => Exportador.ExportarHTML(gridVia, "Viagens"))
);

            Panel painel = CriarPainelCampos(300);

            Lbl("ID:", 30, 25, painel);
            txtViaId = Txt(120, 22, 90, painel, true);

            Lbl("Veículo:", 30, 65, painel);
            cmbViaVeiculo = new ComboBox();
            cmbViaVeiculo.Location = new Point(120, 62);
            cmbViaVeiculo.Width = 320;
            cmbViaVeiculo.Font = new Font("Segoe UI", 11F);
            cmbViaVeiculo.DropDownStyle = ComboBoxStyle.DropDownList;
            painel.Controls.Add(cmbViaVeiculo);

            Lbl("Motorista:", 30, 105, painel);
            cmbViaMotorista = new ComboBox();
            cmbViaMotorista.Location = new Point(120, 102);
            cmbViaMotorista.Width = 320;
            cmbViaMotorista.Font = new Font("Segoe UI", 11F);
            cmbViaMotorista.DropDownStyle = ComboBoxStyle.DropDownList;
            painel.Controls.Add(cmbViaMotorista);

            Lbl("Rota:", 30, 145, painel);
            cmbViaRota = new ComboBox();
            cmbViaRota.Location = new Point(120, 142);
            cmbViaRota.Width = 320;
            cmbViaRota.Font = new Font("Segoe UI", 11F);
            cmbViaRota.DropDownStyle = ComboBoxStyle.DropDownList;
            painel.Controls.Add(cmbViaRota);

            Lbl("Saída:", 30, 185, painel);
            dtpViaSaida = new DateTimePicker();
            dtpViaSaida.Location = new Point(120, 182);
            dtpViaSaida.Width = 200;
            dtpViaSaida.Format = DateTimePickerFormat.Custom;
            dtpViaSaida.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpViaSaida.ShowUpDown = true;
            dtpViaSaida.Font = new Font("Segoe UI", 11F);
            painel.Controls.Add(dtpViaSaida);

            Lbl("Chegada:", 30, 225, painel);
            dtpViaChegada = new DateTimePicker();
            dtpViaChegada.Location = new Point(120, 222);
            dtpViaChegada.Width = 200;
            dtpViaChegada.Format = DateTimePickerFormat.Custom;
            dtpViaChegada.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpViaChegada.ShowUpDown = true;
            dtpViaChegada.Font = new Font("Segoe UI", 11F);
            painel.Controls.Add(dtpViaChegada);

            chkViaEmAndamento = new CheckBox();
            chkViaEmAndamento.Text = "Em andamento (sem data de chegada)";
            chkViaEmAndamento.Location = new Point(340, 222);
            chkViaEmAndamento.AutoSize = true;
            chkViaEmAndamento.Font = new Font("Segoe UI", 10F);
            chkViaEmAndamento.ForeColor = Estilo.TextoEscuro;
            chkViaEmAndamento.Checked = true;
            chkViaEmAndamento.CheckedChanged += (s, e) =>
            {
                // Quando marca "em andamento", desabilita a chegada
                dtpViaChegada.Enabled = !chkViaEmAndamento.Checked;
                cmbViaSituacao.SelectedItem = chkViaEmAndamento.Checked
                    ? "Em Andamento" : "Concluída";
            };
            painel.Controls.Add(chkViaEmAndamento);

            Lbl("Situação:", 30, 265, painel);
            cmbViaSituacao = new ComboBox();
            cmbViaSituacao.Location = new Point(120, 262);
            cmbViaSituacao.Width = 200;
            cmbViaSituacao.Font = new Font("Segoe UI", 11F);
            cmbViaSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbViaSituacao.Items.Add("Em Andamento");
            cmbViaSituacao.Items.Add("Concluída");
            cmbViaSituacao.SelectedIndex = 0;
            painel.Controls.Add(cmbViaSituacao);

            Lbl("Custo Estimado (R$):", 340, 265, painel);
            txtViaCusto = Txt(490, 262, 130, painel, true); // true = só leitura
            txtViaCusto.Text = "0,00";

            // Botão para calcular o custo
            Button btnCalcularCusto = new Button();
            btnCalcularCusto.Text = "Calcular Custo";
            btnCalcularCusto.Location = new Point(630, 261);
            btnCalcularCusto.Width = 120;
            btnCalcularCusto.Height = 28;
            Estilo.Botao(btnCalcularCusto, Estilo.Lilas);
            btnCalcularCusto.Click += (s, e) => CalcularCustoViagem();
            painel.Controls.Add(btnCalcularCusto);

            gridVia = NovaGrid();
            gridVia.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                DataGridViewRow linha = gridVia.Rows[e.RowIndex];

                txtViaId.Text = linha.Cells["viagemID"].Value?.ToString();

                // Como os combos mostram texto, uso o Text pra "achar"
                // o item certo. Não é o jeito mais elegante, mas funciona.
                cmbViaVeiculo.Text = linha.Cells["Veiculo"].Value?.ToString();
                cmbViaMotorista.Text = linha.Cells["Motorista"].Value?.ToString();
                cmbViaRota.Text = linha.Cells["Rota"].Value?.ToString();

                DateTime sd;
                if (DateTime.TryParse(linha.Cells["data_saida"].Value?.ToString(), out sd))
                    dtpViaSaida.Value = sd;

                // A chegada pode ser NULL (viagem em andamento)
                string chegadaTxt = linha.Cells["data_chegada"].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(chegadaTxt))
                {
                    DateTime cd;
                    if (DateTime.TryParse(chegadaTxt, out cd))
                        dtpViaChegada.Value = cd;
                    chkViaEmAndamento.Checked = false;
                }
                else
                {
                    chkViaEmAndamento.Checked = true;
                }

                cmbViaSituacao.Text = linha.Cells["situacao"].Value?.ToString();
            };

            pagina.Enter += (s, e) =>
            {
                ViaCarregarCombos();
                ViaCarregar();
            };

            pagina.Controls.Add(gridVia);
            pagina.Controls.Add(painel);
            pagina.Controls.Add(barra);
            return pagina;
        }

        private void ViaCarregarCombos()
        {
            // ANOTAÇÃO: os combos são preenchidos com SELECT que junta
            // ID + descrição. O ID fica escondido (ValueMember) e o
            // texto aparece pro usuário (DisplayMember).
            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    // Combo de veículos
                    using (MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT veiculoID, CONCAT(placa, ' - ', modelo) AS descricao " +
                        "FROM veiculo ORDER BY placa", conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmbViaVeiculo.DataSource = dt;
                        cmbViaVeiculo.DisplayMember = "descricao";
                        cmbViaVeiculo.ValueMember = "veiculoID";
                    }

                    // Combo de motoristas
                    using (MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT motoristaID, nome AS descricao " +
                        "FROM motorista ORDER BY nome", conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmbViaMotorista.DataSource = dt;
                        cmbViaMotorista.DisplayMember = "descricao";
                        cmbViaMotorista.ValueMember = "motoristaID";
                    }

                    // Combo de rotas
                    using (MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT rotaID, CONCAT(origem, ' → ', destino) AS descricao " +
                        "FROM rota ORDER BY origem", conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmbViaRota.DataSource = dt;
                        cmbViaRota.DisplayMember = "descricao";
                        cmbViaRota.ValueMember = "rotaID";
                    }
                }
            }
            catch (MySqlException) { /* banco fora, deixa vazio */ }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar combos: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViaNovo()
        {
            txtViaId.Clear();
            dtpViaSaida.Value = DateTime.Now;
            dtpViaChegada.Value = DateTime.Now;
            chkViaEmAndamento.Checked = true;
            cmbViaSituacao.SelectedIndex = 0;
            if (cmbViaVeiculo.Items.Count > 0) cmbViaVeiculo.SelectedIndex = 0;
            if (cmbViaMotorista.Items.Count > 0) cmbViaMotorista.SelectedIndex = 0;
            if (cmbViaRota.Items.Count > 0) cmbViaRota.SelectedIndex = 0;
        }

        private void ViaSalvar()
        {
            if (cmbViaVeiculo.SelectedValue == null ||
                cmbViaMotorista.SelectedValue == null ||
                cmbViaRota.SelectedValue == null)
            {
                MessageBox.Show("Preencha Veículo, Motorista e Rota.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"INSERT INTO viagem 
                                   (veiculoID, motoristaID, rotaID, 
                                    data_saida, data_chegada, situacao)
                                   VALUES (@vei, @mot, @rot, @saida, @chegada, @situacao)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@vei", cmbViaVeiculo.SelectedValue);
                        cmd.Parameters.AddWithValue("@mot", cmbViaMotorista.SelectedValue);
                        cmd.Parameters.AddWithValue("@rot", cmbViaRota.SelectedValue);
                        cmd.Parameters.AddWithValue("@saida", dtpViaSaida.Value);

                        // ANOTAÇÃO: se a viagem tá em andamento, a data
                        // de chegada vai como NULL no banco. Pra isso,
                        // a gente passa o valor especial DBNull.Value.
                        if (chkViaEmAndamento.Checked)
                            cmd.Parameters.AddWithValue("@chegada", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@chegada", dtpViaChegada.Value);

                        cmd.Parameters.AddWithValue("@situacao",
                            chkViaEmAndamento.Checked ? "Em Andamento" : "Concluída");

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Viagem registrada!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ViaNovo();
                ViaCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ANOTAÇÃO:
        // Busca a distância, o consumo e o preço do diesel no banco
        // e faz a conta: Custo = (Distancia / Consumo) * Preco
        // ============================================================
        private void CalcularCustoViagem()
        {
            if (cmbViaVeiculo.SelectedValue == null || cmbViaRota.SelectedValue == null)
            {
                MessageBox.Show("Selecione o Veículo e a Rota primeiro.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    // 1. Pega o Consumo do Veículo
                    decimal consumo = 0;
                    using (MySqlCommand cmd = new MySqlCommand("SELECT consumo_medio FROM veiculo WHERE veiculoID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", cmbViaVeiculo.SelectedValue);
                        consumo = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                    // 2. Pega a Distância da Rota
                    decimal distancia = 0;
                    using (MySqlCommand cmd = new MySqlCommand("SELECT distancia FROM rota WHERE rotaID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", cmbViaRota.SelectedValue);
                        distancia = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                    // 3. Pega o preço mais recente do combustível
                    decimal precoCombustivel = 0;
                    using (MySqlCommand cmd = new MySqlCommand("SELECT preco FROM preco_combustivel ORDER BY data_consulta DESC LIMIT 1", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null) precoCombustivel = Convert.ToDecimal(result);
                    }

                    // Validações de segurança (pra não dividir por zero)
                    if (consumo <= 0) { MessageBox.Show("O veículo não tem consumo médio cadastrado.", "Erro"); return; }
                    if (precoCombustivel <= 0) { MessageBox.Show("Nenhum preço de combustível cadastrado no sistema.", "Erro"); return; }

                    // 4. Faz a conta
                    decimal custoTotal = (distancia / consumo) * precoCombustivel;

                    // 5. Mostra na tela
                    txtViaCusto.Text = custoTotal.ToString("N2");

                    MessageBox.Show($"Cálculo realizado:\n\n" +
                                    $"Distância: {distancia:N0} km\n" +
                                    $"Consumo do Veículo: {consumo:N1} km/L\n" +
                                    $"Preço do Diesel: R$ {precoCombustivel:N2}\n\n" +
                                    $"Custo Estimado: R$ {custoTotal:N2}",
                                    "Detalhes do Cálculo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao calcular: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViaEditar()
        {
            if (string.IsNullOrEmpty(txtViaId.Text))
            {
                MessageBox.Show("Selecione uma viagem.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"UPDATE viagem 
                                   SET veiculoID = @vei,
                                       motoristaID = @mot,
                                       rotaID = @rot,
                                       data_saida = @saida,
                                       data_chegada = @chegada,
                                       situacao = @situacao
                                   WHERE viagemID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtViaId.Text);
                        cmd.Parameters.AddWithValue("@vei", cmbViaVeiculo.SelectedValue);
                        cmd.Parameters.AddWithValue("@mot", cmbViaMotorista.SelectedValue);
                        cmd.Parameters.AddWithValue("@rot", cmbViaRota.SelectedValue);
                        cmd.Parameters.AddWithValue("@saida", dtpViaSaida.Value);

                        if (chkViaEmAndamento.Checked)
                            cmd.Parameters.AddWithValue("@chegada", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@chegada", dtpViaChegada.Value);

                        cmd.Parameters.AddWithValue("@situacao",
                            chkViaEmAndamento.Checked ? "Em Andamento" : "Concluída");

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Viagem atualizada!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ViaNovo();
                ViaCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViaExcluir()
        {
            if (!PodeExcluir()) return;


            if (string.IsNullOrEmpty(txtViaId.Text))
            {
                MessageBox.Show("Selecione uma viagem.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resp = MessageBox.Show(
                "Excluir esta viagem?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM viagem WHERE viagemID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtViaId.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Viagem excluída.", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ViaNovo();
                ViaCarregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViaCarregar()
        {
            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    // JOIN pra trazer os nomes em vez de só os IDs.
                    // Fica mais amigável pro usuário final.
                    string sql = @"
                        SELECT v.viagemID,
                               ve.placa AS Veiculo,
                               mo.nome AS Motorista,
                               CONCAT(r.origem, ' → ', r.destino) AS Rota,
                               v.data_saida,
                               v.data_chegada,
                               v.situacao
                        FROM viagem v
                        JOIN veiculo ve ON ve.veiculoID = v.veiculoID
                        JOIN motorista mo ON mo.motoristaID = v.motoristaID
                        JOIN rota r ON r.rotaID = v.rotaID
                        ORDER BY v.viagemID DESC";

                    using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable tabela = new DataTable();
                        da.Fill(tabela);
                        gridVia.DataSource = tabela;
                    }
                }
            }
            catch (MySqlException) { gridVia.DataSource = null; }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ═══════════════ ABA RASTREAMENTO ══════════════════════════
        // ============================================================
        // ANOTAÇÃO:
        // Mostra cada viagem em andamento num "card" colorido.
        // A cor muda conforme o tempo que a viagem já tá rolando:
        //   - Verde (menta) = menos de 4h (normal)
        //   - Lilás = entre 4h e 8h (atenção)
        //   - Índigo escuro = mais de 8h (atrasada)
        //
        // Tem um timer que atualiza os tempos a cada 30 segundos,
        // então o painel fica "vivo" durante a apresentação.
        // ============================================================

        private TabPage CriarAbaRastreamento()
        {
            TabPage pagina = NovaAba("Rastreamento");

            // ----- Barra de ferramentas no topo -----
            ToolStrip barra = CriarToolbar(
    ("Filtrar", Icones.Filtro, (s, e) => FiltrarViagens()),
    ("Limpar", Icones.Limpar, (s, e) => { LimparFiltros(); FiltrarViagens(); }
            ),
    ("CSV", Icones.Csv, (s, e) => Exportador.ExportarCSV(gridConsulta, "Monitoramento")),
    ("Relatório", Icones.Pdf, (s, e) => Exportador.ExportarHTML(gridConsulta, "Monitoramento"))
);

            // ----- Painel com legenda -----
            Panel painelTopo = new Panel();
            painelTopo.Dock = DockStyle.Top;
            painelTopo.Height = 60;
            painelTopo.BackColor = Color.White;
            painelTopo.Padding = new Padding(20);

            Label lblTitulo = new Label();
            lblTitulo.Text = "📍 Acompanhamento de Viagens em Andamento";
            lblTitulo.Font = new Font("Segoe UI Semibold", 14F);
            lblTitulo.ForeColor = Estilo.IndigoEscuro;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(20, 15);
            painelTopo.Controls.Add(lblTitulo);

            Label lblLegenda = new Label();
            lblLegenda.Text = "🟢 Normal (< 4h)    🟣 Atenção (4-8h)    🔵 Atrasada (> 8h)";
            lblLegenda.Font = new Font("Segoe UI", 10F);
            lblLegenda.ForeColor = Color.Gray;
            lblLegenda.AutoSize = true;
            lblLegenda.Location = new Point(20, 40);
            painelTopo.Controls.Add(lblLegenda);

            // ----- Painel com scroll (onde ficam os cards) -----
            Panel painelCards = new Panel();
            painelCards.Dock = DockStyle.Fill;
            painelCards.BackColor = Estilo.MentaBemClaro;
            painelCards.AutoScroll = true;
            painelCards.Padding = new Padding(20);
            painelCards.Name = "painelCardsRastreamento"; // nome pra achar depois

            // Adiciona os controles na ordem certa
            pagina.Controls.Add(painelCards);
            pagina.Controls.Add(painelTopo);
            pagina.Controls.Add(barra);

            // ANOTAÇÃO: timer que atualiza a cada 30 segundos.
            // Faz os "Saiu há X" mudarem em tempo real durante a apresentação.
            System.Windows.Forms.Timer timerAtualiza = new System.Windows.Forms.Timer(); timerAtualiza.Interval = 30000; // 30 segundos
            timerAtualiza.Tick += (s, e) =>
            {
                // Só atualiza se a aba tá visível (economiza recurso)
                if (abas.SelectedTab == pagina)
                    CarregarRastreamento();
            };
            timerAtualiza.Start();

            // Guarda referência pro timer não ser destruído pelo Garbage Collector
            pagina.Tag = timerAtualiza;

            // Carrega ao entrar na aba
            pagina.Enter += (s, e) => CarregarRastreamento();

            return pagina;
        }

        // ============================================================
        // Busca as viagens em andamento e monta os cards
        // ============================================================
        private void CarregarRastreamento()
        {
            // Acha o painel pelo nome (truque pra achar sem variável global)
            Control[] encontrado = this.Controls.Find("painelCardsRastreamento", true);
            if (encontrado.Length == 0) return;

            Panel painelCards = (Panel)encontrado[0];
            painelCards.Controls.Clear();

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"
                SELECT v.viagemID,
                       ve.placa,
                       mo.nome AS motorista,
                       CONCAT(r.origem, ' → ', r.destino) AS rota,
                       v.data_saida
                FROM viagem v
                JOIN veiculo ve ON ve.veiculoID = v.veiculoID
                JOIN motorista mo ON mo.motoristaID = v.motoristaID
                JOIN rota r ON r.rotaID = v.rotaID
                WHERE v.situacao = 'Em Andamento'
                ORDER BY v.data_saida ASC";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader rd = cmd.ExecuteReader())
                    {
                        int posicaoY = 10;
                        int total = 0;

                        while (rd.Read())
                        {
                            string placa = rd.GetString("placa");
                            string motorista = rd.GetString("motorista");
                            string rota = rd.GetString("rota");
                            DateTime saida = rd.GetDateTime("data_saida");

                            // Calcula o tempo decorrido
                            TimeSpan decorrido = DateTime.Now - saida;
                            int horas = (int)decorrido.TotalHours;
                            int minutos = decorrido.Minutes;

                            // Define a cor conforme o tempo
                            Color corStatus;
                            string iconeStatus;
                            string avisoAtraso = "";

                            if (horas < 4)
                            {
                                corStatus = Estilo.MentaEscuro;
                                iconeStatus = "🟢";
                            }
                            else if (horas < 8)
                            {
                                corStatus = Estilo.LilasEscuro;
                                iconeStatus = "🟣";
                            }
                            else
                            {
                                corStatus = Estilo.IndigoEscuro;
                                iconeStatus = "🔵";
                                avisoAtraso = " (ATRASADA!)";
                            }

                            // Cria o card
                            Panel card = new Panel();
                            card.Location = new Point(10, posicaoY);
                            card.Size = new Size(painelCards.Width - 40, 100);
                            card.BackColor = Color.White;

                            // Borda e barra colorida (no Paint)
                            Color corFinal = corStatus;
                            card.Paint += (s, e) =>
                            {
                                Rectangle r = card.ClientRectangle;
                                r.Width -= 1;
                                r.Height -= 1;
                                using (Pen caneta = new Pen(Estilo.LilasClaro, 1))
                                    e.Graphics.DrawRectangle(caneta, r);

                                using (SolidBrush pincel = new SolidBrush(corFinal))
                                    e.Graphics.FillRectangle(pincel, 0, 0, 8, card.Height);
                            };

                            // Linha 1: placa + motorista
                            Label lblLinha1 = new Label();
                            lblLinha1.Text = iconeStatus + "  " + placa + "  ·  " + motorista;
                            lblLinha1.Font = new Font("Segoe UI Semibold", 13F);
                            lblLinha1.ForeColor = Estilo.IndigoEscuro;
                            lblLinha1.AutoSize = true;
                            lblLinha1.Location = new Point(25, 15);
                            card.Controls.Add(lblLinha1);

                            // Linha 2: rota
                            Label lblLinha2 = new Label();
                            lblLinha2.Text = rota;
                            lblLinha2.Font = new Font("Segoe UI", 11F);
                            lblLinha2.ForeColor = Estilo.TextoEscuro;
                            lblLinha2.AutoSize = true;
                            lblLinha2.Location = new Point(25, 45);
                            card.Controls.Add(lblLinha2);

                            // Linha 3: tempo decorrido
                            Label lblLinha3 = new Label();
                            lblLinha3.Text = "⏱️  Saiu há " + horas + "h " + minutos.ToString("00") + "min" + avisoAtraso;
                            lblLinha3.Font = new Font("Segoe UI Semibold", 10F);
                            lblLinha3.ForeColor = corStatus;
                            lblLinha3.AutoSize = true;
                            lblLinha3.Location = new Point(25, 70);
                            card.Controls.Add(lblLinha3);

                            painelCards.Controls.Add(card);
                            posicaoY += 115;
                            total++;
                        }

                        // Se não tem viagem em andamento, mostra mensagem
                        if (total == 0)
                        {
                            Label lblVazio = new Label();
                            lblVazio.Text = "✅ Nenhuma viagem em andamento no momento.";
                            lblVazio.Font = new Font("Segoe UI", 14F, FontStyle.Italic);
                            lblVazio.ForeColor = Color.Gray;
                            lblVazio.AutoSize = true;
                            lblVazio.Location = new Point(30, 30);
                            painelCards.Controls.Add(lblVazio);
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                // Banco fora, deixa o painel vazio
            }
            catch (Exception ex)
            {
                Log.Erro("Erro no rastreamento", ex);
            }
        }

        // ============================================================
        // ═══════════════ ABA CONSULTAR / MONITORAR ═════════════════
        // ============================================================
        private TabPage CriarAbaConsulta()
        {
            TabPage pagina = NovaAba("Monitoramento");

            ToolStrip barra = CriarToolbar(
                ("Filtrar", Icones.Filtro, (s, e) => FiltrarViagens()),
                ("Limpar", Icones.Limpar, (s, e) => { LimparFiltros(); FiltrarViagens(); }
            ),
                ("CSV", Icones.Csv, (s, e) => Exportador.ExportarCSV(gridConsulta, "Monitoramento")),
                ("Relatório", Icones.Pdf, (s, e) => Exportador.ExportarHTML(gridConsulta, "Monitoramento"))
            );


            Panel painel = CriarPainelCampos(160);

            Lbl("Status:", 30, 25, painel);
            cmbFiltroStatus = new ComboBox();
            cmbFiltroStatus.Location = new Point(110, 22);
            cmbFiltroStatus.Width = 180;
            cmbFiltroStatus.Font = new Font("Segoe UI", 11F);
            cmbFiltroStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroStatus.Items.Add("Todos");
            cmbFiltroStatus.Items.Add("Em Andamento");
            cmbFiltroStatus.Items.Add("Concluída");
            cmbFiltroStatus.SelectedIndex = 0;
            painel.Controls.Add(cmbFiltroStatus);

            Lbl("De:", 320, 25, painel);
            dtpFiltroDe = new DateTimePicker();
            dtpFiltroDe.Location = new Point(370, 22);
            dtpFiltroDe.Width = 160;
            dtpFiltroDe.Format = DateTimePickerFormat.Short;
            dtpFiltroDe.Font = new Font("Segoe UI", 11F);
            dtpFiltroDe.Value = DateTime.Now.AddMonths(-1);
            painel.Controls.Add(dtpFiltroDe);

            Lbl("Até:", 560, 25, painel);
            dtpFiltroAte = new DateTimePicker();
            dtpFiltroAte.Location = new Point(610, 22);
            dtpFiltroAte.Width = 160;
            dtpFiltroAte.Format = DateTimePickerFormat.Short;
            dtpFiltroAte.Font = new Font("Segoe UI", 11F);
            painel.Controls.Add(dtpFiltroAte);

            Label lblTitulo = new Label();
            lblTitulo.Text = "Resultados e Acompanhamento";
            lblTitulo.Font = new Font("Segoe UI Semibold", 12F);
            lblTitulo.ForeColor = Estilo.IndigoEscuro;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(30, 75);
            painel.Controls.Add(lblTitulo);

            // Legenda de cores
            Label lblLegenda = new Label();
            lblLegenda.Text = "🟢 Normal (< 4h)    🟣 Atenção (4-8h)    🔵 Atrasada (> 8h)";
            lblLegenda.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblLegenda.ForeColor = Color.Gray;
            lblLegenda.AutoSize = true;
            lblLegenda.Location = new Point(30, 100);
            painel.Controls.Add(lblLegenda);

            gridConsulta = NovaGrid();
            pagina.Enter += (s, e) => FiltrarViagens();

            pagina.Controls.Add(gridConsulta);
            pagina.Controls.Add(painel);
            pagina.Controls.Add(barra);
            return pagina;
        }

        private void LimparFiltros()
        {
            cmbFiltroStatus.SelectedIndex = 0;
            dtpFiltroDe.Value = DateTime.Now.AddMonths(-1);
            dtpFiltroAte.Value = DateTime.Now;
        }

        private void FiltrarViagens()
        {
            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"
                        SELECT v.viagemID AS ID,
                               ve.placa AS Veiculo,
                               mo.nome AS Motorista,
                               CONCAT(r.origem, ' → ', r.destino) AS Rota,
                               v.data_saida AS Saida,
                               v.data_chegada AS Chegada,
                               v.situacao AS Status
                        FROM viagem v
                        JOIN veiculo ve ON ve.veiculoID = v.veiculoID
                        JOIN motorista mo ON mo.motoristaID = v.motoristaID
                        JOIN rota r ON r.rotaID = v.rotaID
                        WHERE 1=1";

                    if (cmbFiltroStatus.SelectedIndex == 1)
                        sql += " AND v.situacao = 'Em Andamento'";
                    else if (cmbFiltroStatus.SelectedIndex == 2)
                        sql += " AND v.situacao = 'Concluída'";

                    sql += " AND DATE(v.data_saida) BETWEEN @de AND @ate";
                    sql += " ORDER BY v.viagemID DESC";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@de", dtpFiltroDe.Value.Date);
                        cmd.Parameters.AddWithValue("@ate", dtpFiltroAte.Value.Date);

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            DataTable tabela = new DataTable();
                            da.Fill(tabela);
                            gridConsulta.DataSource = tabela;
                        }
                    }
                }

                // Colorir as linhas de viagens em andamento
                foreach (DataGridViewRow linha in gridConsulta.Rows)
                {
                    string status = linha.Cells["Status"].Value?.ToString();

                    if (status == "Em Andamento")
                    {
                        DateTime saida = Convert.ToDateTime(linha.Cells["Saida"].Value);
                        TimeSpan tempoDecorrido = DateTime.Now - saida;
                        double horas = tempoDecorrido.TotalHours;

                        Color corLinha;
                        if (horas < 4)
                            corLinha = Estilo.MentaEscuro;
                        else if (horas < 8)
                            corLinha = Estilo.LilasEscuro;
                        else
                            corLinha = Estilo.IndigoEscuro;

                        linha.DefaultCellStyle.BackColor = corLinha;
                        linha.DefaultCellStyle.ForeColor = Color.White;
                        linha.DefaultCellStyle.SelectionBackColor = corLinha;
                    }
                }
            }
            catch (MySqlException)
            {
                gridConsulta.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro no filtro: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ============================================================
        // MÉTODO DE LOGOUT (Trocar de usuário)
        // ============================================================
        private void FazerLogout()
        {
            DialogResult resp = MessageBox.Show(
                "Deseja realmente sair da sua conta?",
                "Confirmar Saída",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resp == DialogResult.Yes)
            {
                Session.Usuario = "";
                Session.Perfil = "";
                Log.Info("Logout realizado pelo usuário.");

                // Reinicia o sistema, caindo na tela de login
                Application.Restart();
            }
        }
    }

    // Classe auxiliar pra pintar o menu suspenso
    public class CustomColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Estilo.Lilas;
        public override Color MenuItemBorder => Estilo.Lilas;
        public override Color ToolStripDropDownBackground => Estilo.Indigo;
    }
}