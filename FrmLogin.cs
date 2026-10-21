using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MySqlConnector;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Tela de login com hash SHA256, bloqueio após 5 tentativas,
    // log de auditoria e gravação da Session (usuário logado).
    // ============================================================

    public class FrmLogin : Form
    {
        private TextBox txtUsuario;
        private TextBox txtSenha;
        private Button btnEntrar;
        private Button btnCadastrar;

        public FrmLogin()
        {
            this.Text = "Rota+ · Login";
            this.ClientSize = new Size(900, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Estilo.MentaBemClaro;
            this.Font = new Font("Segoe UI", 10F);

            // ============================================================
            // PAINEL ESQUERDO
            // ============================================================
            Panel painelEsq = new Panel();
            painelEsq.Dock = DockStyle.Left;
            painelEsq.Width = 400;
            painelEsq.BackColor = Estilo.Indigo;
            this.Controls.Add(painelEsq);

            painelEsq.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush b = new SolidBrush(Estilo.Lilas))
                    e.Graphics.FillEllipse(b, -70, -70, 200, 200);
                using (SolidBrush b = new SolidBrush(Estilo.Menta))
                    e.Graphics.FillEllipse(b, 320, 460, 160, 160);
            };

            Label lblRota = new Label();
            lblRota.Text = "Rota";
            lblRota.Font = new Font("Segoe UI Black", 56F);
            lblRota.ForeColor = Estilo.Menta;
            lblRota.AutoSize = true;
            lblRota.Location = new Point(50, 115);
            lblRota.BackColor = Color.Transparent;
            painelEsq.Controls.Add(lblRota);

            Label lblMais = new Label();
            lblMais.Text = "+";
            lblMais.Font = new Font("Segoe UI Black", 48F);
            lblMais.ForeColor = Estilo.Lilas;
            lblMais.AutoSize = false;
            lblMais.Size = new Size(70, 90);
            lblMais.TextAlign = ContentAlignment.MiddleCenter;
            lblMais.Location = new Point(240, 115);
            lblMais.BackColor = Color.Transparent;
            painelEsq.Controls.Add(lblMais);
            lblMais.BringToFront();

            Label lblSub = new Label();
            lblSub.Text = "Sistema de\nGerenciamento\nLogístico";
            lblSub.Font = new Font("Segoe UI Semibold", 20F);
            lblSub.ForeColor = Color.White;
            lblSub.AutoSize = true;
            lblSub.Location = new Point(50, 240);
            lblSub.BackColor = Color.Transparent;
            painelEsq.Controls.Add(lblSub);

            Label lblSlogan = new Label();
            lblSlogan.Text = "🚚  Rastreie. Analise. Entregue.";
            lblSlogan.Font = new Font("Segoe UI", 11F);
            lblSlogan.ForeColor = Estilo.LilasBemClaro;
            lblSlogan.AutoSize = true;
            lblSlogan.Location = new Point(50, 400);
            lblSlogan.BackColor = Color.Transparent;
            painelEsq.Controls.Add(lblSlogan);

            Label lblCopy = new Label();
            lblCopy.Text = "© 2026 · Rota+";
            lblCopy.Font = new Font("Segoe UI", 9F);
            lblCopy.ForeColor = Estilo.LilasBemClaro;
            lblCopy.AutoSize = true;
            lblCopy.Location = new Point(50, 530);
            lblCopy.BackColor = Color.Transparent;
            painelEsq.Controls.Add(lblCopy);

            // ============================================================
            // CARD DE LOGIN
            // ============================================================
            Panel card = new Panel();
            card.Size = new Size(400, 430);
            card.Location = new Point(465, 75);
            card.BackColor = Color.White;
            this.Controls.Add(card);

            card.Paint += (s, e) =>
            {
                Rectangle r = card.ClientRectangle;
                r.Width -= 1;
                r.Height -= 1;
                using (Pen caneta = new Pen(Estilo.LilasClaro, 1))
                    e.Graphics.DrawRectangle(caneta, r);
            };

            Label lblBoasVindas = new Label();
            lblBoasVindas.Text = "Bem-vindo(a) 👋";
            lblBoasVindas.Font = new Font("Segoe UI Semibold", 22F);
            lblBoasVindas.ForeColor = Estilo.IndigoEscuro;
            lblBoasVindas.AutoSize = true;
            lblBoasVindas.Location = new Point(40, 35);
            card.Controls.Add(lblBoasVindas);

            Label lblInstrucao = new Label();
            lblInstrucao.Text = "Entre com suas credenciais para continuar";
            lblInstrucao.Font = new Font("Segoe UI", 10F);
            lblInstrucao.ForeColor = Estilo.TextoEscuro;
            lblInstrucao.AutoSize = true;
            lblInstrucao.Location = new Point(40, 83);
            card.Controls.Add(lblInstrucao);

            Label lblUsuario = new Label();
            lblUsuario.Text = "Usuário";
            lblUsuario.Font = new Font("Segoe UI Semibold", 10F);
            lblUsuario.ForeColor = Estilo.TextoEscuro;
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(40, 135);
            card.Controls.Add(lblUsuario);

            txtUsuario = new TextBox();
            txtUsuario.Location = new Point(40, 160);
            txtUsuario.Width = 320;
            Estilo.TextBox(txtUsuario);
            card.Controls.Add(txtUsuario);

            Label lblSenha = new Label();
            lblSenha.Text = "Senha";
            lblSenha.Font = new Font("Segoe UI Semibold", 10F);
            lblSenha.ForeColor = Estilo.TextoEscuro;
            lblSenha.AutoSize = true;
            lblSenha.Location = new Point(40, 210);
            card.Controls.Add(lblSenha);

            // BOTÃO OLHINHO (Mostrar/Ocultar senha)
            CheckBox chkMostrarSenha = new CheckBox();
            chkMostrarSenha.Text = "Mostrar senha";
            chkMostrarSenha.Location = new Point(40, 265); // Fica logo abaixo da senha
            chkMostrarSenha.AutoSize = true;
            chkMostrarSenha.Font = new Font("Segoe UI", 9F);
            chkMostrarSenha.ForeColor = Estilo.TextoEscuro;
            chkMostrarSenha.CheckedChanged += (s, e) =>
            {
                txtSenha.UseSystemPasswordChar = !chkMostrarSenha.Checked;
            };
            card.Controls.Add(chkMostrarSenha);

            txtSenha = new TextBox();
            txtSenha.Location = new Point(40, 235);
            txtSenha.Width = 320;
            txtSenha.UseSystemPasswordChar = true;
            Estilo.TextBox(txtSenha);
            card.Controls.Add(txtSenha);

            btnEntrar = new Button();
            btnEntrar.Text = "Entrar";
            btnEntrar.Location = new Point(40, 305);
            btnEntrar.Width = 320;
            btnEntrar.TextAlign = ContentAlignment.MiddleCenter;
            Estilo.Botao(btnEntrar, Estilo.Indigo);
            btnEntrar.Click += BtnEntrar_Click;
            card.Controls.Add(btnEntrar);

            btnCadastrar = new Button();
            btnCadastrar.Text = "Criar nova conta";
            btnCadastrar.Location = new Point(40, 355);
            btnCadastrar.Width = 320;
            btnCadastrar.Height = 34;
            btnCadastrar.FlatStyle = FlatStyle.Flat;
            btnCadastrar.BackColor = Color.Transparent;
            btnCadastrar.ForeColor = Estilo.LilasEscuro;
            btnCadastrar.Font = new Font("Segoe UI Semibold", 10F);
            btnCadastrar.Cursor = Cursors.Hand;
            btnCadastrar.FlatAppearance.BorderSize = 0;
            btnCadastrar.FlatAppearance.MouseOverBackColor = Estilo.LilasBemClaro;
            btnCadastrar.Click += BtnCadastrar_Click;
            card.Controls.Add(btnCadastrar);

            Label lblDica = new Label();
            lblDica.Text = "admin / 123";
            lblDica.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblDica.ForeColor = Color.Gray;
            lblDica.AutoSize = true;
            lblDica.Location = new Point(40, 398);
            card.Controls.Add(lblDica);

            this.AcceptButton = btnEntrar;
        }

        // ============================================================
        // BOTÃO ENTRAR
        // ============================================================
        private void BtnEntrar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                MessageBox.Show("Preencha usuário e senha.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loginDigitado = txtUsuario.Text.Trim();
            Log.Info("Tentativa de login: " + loginDigitado);

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"SELECT usuarioID, senha, perfil, tentativas, bloqueado_ate 
                                   FROM usuario 
                                   WHERE login = @login";

                    int id = 0;
                    string hashDoBanco = null;
                    string perfil = null;
                    int tentativas = 0;
                    DateTime? bloqueadoAte = null;

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", loginDigitado);

                        using (MySqlDataReader rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                Log.Info("Login falhou: usuário inexistente '" + loginDigitado + "'");
                                MessageBox.Show("Usuário ou senha inválidos.", "Erro de login",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            id = rd.GetInt32("usuarioID");
                            hashDoBanco = rd.GetString("senha");
                            perfil = rd.GetString("perfil");
                            tentativas = rd.GetInt32("tentativas");

                            if (!rd.IsDBNull(rd.GetOrdinal("bloqueado_ate")))
                                bloqueadoAte = rd.GetDateTime("bloqueado_ate");
                        }
                    }

                    // Tá bloqueado?
                    if (bloqueadoAte != null && bloqueadoAte.Value > DateTime.Now)
                    {
                        int minutosRestantes = (int)(bloqueadoAte.Value - DateTime.Now).TotalMinutes;
                        if (minutosRestantes < 1) minutosRestantes = 1;

                        Log.Erro("Login bloqueado: " + loginDigitado +
                                 " (faltam " + minutosRestantes + " min)");

                        MessageBox.Show(
                            "Usuário bloqueado por tentativas erradas.\n" +
                            "Tente novamente em " + minutosRestantes + " minuto(s).",
                            "Bloqueado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Senha confere?
                    if (Security.SenhaConfere(txtSenha.Text, hashDoBanco))
                    {
                        using (MySqlCommand cmdReset = new MySqlCommand(
                            "UPDATE usuario SET tentativas = 0, bloqueado_ate = NULL WHERE usuarioID = @id", conn))
                        {
                            cmdReset.Parameters.AddWithValue("@id", id);
                            cmdReset.ExecuteNonQuery();
                        }

                        // ANOTAÇÃO: guarda o usuário na Session
                        Session.Usuario = loginDigitado;
                        Session.Perfil = perfil;

                        Log.Info("Login OK: " + loginDigitado + " (" + perfil + ")");

                        this.Hide();
                        FrmPrincipal tela = new FrmPrincipal();
                        tela.ShowDialog();
                        this.Close();
                        return;
                    }

                    // Senha errada
                    tentativas++;

                    if (tentativas >= 5)
                    {
                        using (MySqlCommand cmdBloq = new MySqlCommand(
                            "UPDATE usuario SET tentativas = 0, bloqueado_ate = @bloq WHERE usuarioID = @id", conn))
                        {
                            cmdBloq.Parameters.AddWithValue("@bloq", DateTime.Now.AddMinutes(15));
                            cmdBloq.Parameters.AddWithValue("@id", id);
                            cmdBloq.ExecuteNonQuery();
                        }

                        Log.Erro("Usuário " + loginDigitado + " bloqueado após 5 tentativas");

                        MessageBox.Show(
                            "Muitas tentativas erradas!\n" +
                            "Usuário bloqueado por 15 minutos.",
                            "Bloqueado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        using (MySqlCommand cmdUp = new MySqlCommand(
                            "UPDATE usuario SET tentativas = @t WHERE usuarioID = @id", conn))
                        {
                            cmdUp.Parameters.AddWithValue("@t", tentativas);
                            cmdUp.Parameters.AddWithValue("@id", id);
                            cmdUp.ExecuteNonQuery();
                        }

                        int restantes = 5 - tentativas;
                        Log.Info("Senha incorreta para '" + loginDigitado +
                                 "' (tentativa " + tentativas + "/5)");

                        MessageBox.Show(
                            "Usuário ou senha inválidos.\n" +
                            "Restam " + restantes + " tentativa(s) antes do bloqueio.",
                            "Erro de login",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    txtSenha.Clear();
                    txtSenha.Focus();
                }
            }
            catch (MySqlException ex)
            {
                Log.Erro("Erro de conexão no login", ex);

                MessageBox.Show(
                    "Não consegui conectar no banco de dados. 😕\n\n" +
                    "Verifica se:\n" +
                    "• O MySQL tá instalado e rodando\n" +
                    "• A senha no appsettings.json tá certa\n" +
                    "• O banco 'projeto_logistica' já foi criado\n\n" +
                    "Se o problema continuar, chama o suporte! 📞",
                    "Erro de conexão",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Log.Erro("Erro inesperado no login", ex);
                MessageBox.Show("Erro inesperado:\n\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCadastrar_Click(object? sender, EventArgs e)
        {
            FrmCadastro tela = new FrmCadastro();
            tela.ShowDialog();
        }
    }
}

