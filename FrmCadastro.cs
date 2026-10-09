using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MySqlConnector;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Tela de cadastro de novos usuários. Senha vai pro banco
    // como hash SHA256 e o cadastro é registrado no Log.
    // ============================================================

    public class FrmCadastro : Form
    {
        private TextBox txtLogin;
        private TextBox txtSenha;
        private TextBox txtConfirma;
        private Button btnCadastrar;
        private Button btnVoltar;

        public FrmCadastro()
        {
            this.Text = "Rota+ · Criar Conta";
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
            lblSub.Text = "Crie sua conta\ne comece a usar\nnossos serviços";
            lblSub.Font = new Font("Segoe UI Semibold", 18F);
            lblSub.ForeColor = Color.White;
            lblSub.AutoSize = true;
            lblSub.Location = new Point(50, 240);
            lblSub.BackColor = Color.Transparent;
            painelEsq.Controls.Add(lblSub);

            Label lblCopy = new Label();
            lblCopy.Text = "© 2026 · Rota+";
            lblCopy.Font = new Font("Segoe UI", 9F);
            lblCopy.ForeColor = Estilo.LilasBemClaro;
            lblCopy.AutoSize = true;
            lblCopy.Location = new Point(50, 530);
            lblCopy.BackColor = Color.Transparent;
            painelEsq.Controls.Add(lblCopy);

            // ============================================================
            // CARD DE CADASTRO
            // ============================================================
            Panel card = new Panel();
            card.Size = new Size(400, 470);
            card.Location = new Point(465, 55);
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

            Label lblTitulo = new Label();
            lblTitulo.Text = "Criar conta ✨";
            lblTitulo.Font = new Font("Segoe UI Semibold", 22F);
            lblTitulo.ForeColor = Estilo.IndigoEscuro;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(40, 30);
            card.Controls.Add(lblTitulo);

            Label lblInstrucao = new Label();
            lblInstrucao.Text = "Preencha os dados para se cadastrar";
            lblInstrucao.Font = new Font("Segoe UI", 10F);
            lblInstrucao.ForeColor = Estilo.TextoEscuro;
            lblInstrucao.AutoSize = true;
            lblInstrucao.Location = new Point(40, 78);
            card.Controls.Add(lblInstrucao);

            // ----- LOGIN -----
            Label lblLogin = new Label();
            lblLogin.Text = "Usuário";
            lblLogin.Font = new Font("Segoe UI Semibold", 10F);
            lblLogin.ForeColor = Estilo.TextoEscuro;
            lblLogin.AutoSize = true;
            lblLogin.Location = new Point(40, 125);
            card.Controls.Add(lblLogin);

            txtLogin = new TextBox();
            txtLogin.Location = new Point(40, 150);
            txtLogin.Width = 320;
            Estilo.TextBox(txtLogin);
            card.Controls.Add(txtLogin);

            // ----- SENHA -----
            Label lblSenha = new Label();
            lblSenha.Text = "Senha";
            lblSenha.Font = new Font("Segoe UI Semibold", 10F);
            lblSenha.ForeColor = Estilo.TextoEscuro;
            lblSenha.AutoSize = true;
            lblSenha.Location = new Point(40, 195);
            card.Controls.Add(lblSenha);

            txtSenha = new TextBox();
            txtSenha.Location = new Point(40, 220);
            txtSenha.Width = 320;
            txtSenha.UseSystemPasswordChar = true;
            Estilo.TextBox(txtSenha);
            card.Controls.Add(txtSenha);

            // ----- CONFIRMAR -----
            Label lblConfirma = new Label();
            lblConfirma.Text = "Confirmar senha";
            lblConfirma.Font = new Font("Segoe UI Semibold", 10F);
            lblConfirma.ForeColor = Estilo.TextoEscuro;
            lblConfirma.AutoSize = true;
            lblConfirma.Location = new Point(40, 265);
            card.Controls.Add(lblConfirma);

            txtConfirma = new TextBox();
            txtConfirma.Location = new Point(40, 290);
            txtConfirma.Width = 320;
            txtConfirma.UseSystemPasswordChar = true;
            Estilo.TextBox(txtConfirma);
            card.Controls.Add(txtConfirma);

            // ----- BOTÃO CADASTRAR -----
            btnCadastrar = new Button();
            btnCadastrar.Text = "Cadastrar";
            btnCadastrar.Location = new Point(40, 350);
            btnCadastrar.Width = 320;
            btnCadastrar.TextAlign = ContentAlignment.MiddleCenter;
            Estilo.Botao(btnCadastrar, Estilo.Indigo);
            btnCadastrar.Click += BtnCadastrar_Click;
            card.Controls.Add(btnCadastrar);

            // ----- BOTÃO VOLTAR -----
            btnVoltar = new Button();
            btnVoltar.Text = "← Voltar pro login";
            btnVoltar.Location = new Point(40, 400);
            btnVoltar.Width = 320;
            btnVoltar.Height = 34;
            btnVoltar.FlatStyle = FlatStyle.Flat;
            btnVoltar.BackColor = Color.Transparent;
            btnVoltar.ForeColor = Estilo.LilasEscuro;
            btnVoltar.Font = new Font("Segoe UI Semibold", 10F);
            btnVoltar.Cursor = Cursors.Hand;
            btnVoltar.FlatAppearance.BorderSize = 0;
            btnVoltar.FlatAppearance.MouseOverBackColor = Estilo.LilasBemClaro;
            btnVoltar.Click += (s, e) => this.Close();
            card.Controls.Add(btnVoltar);

            this.AcceptButton = btnCadastrar;
        }

        // ============================================================
        // BOTÃO CADASTRAR
        // ============================================================
        private void BtnCadastrar_Click(object sender, EventArgs e)
        {
            // ----- Validações -----
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Preencha o nome de usuário.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                MessageBox.Show("Preencha a senha.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSenha.Focus();
                return;
            }

            if (txtSenha.Text.Length < 4)
            {
                MessageBox.Show("A senha precisa ter no mínimo 4 caracteres.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSenha.Focus();
                return;
            }

            if (txtSenha.Text != txtConfirma.Text)
            {
                MessageBox.Show("As senhas não conferem.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirma.Clear();
                txtConfirma.Focus();
                return;
            }

            string loginDigitado = txtLogin.Text.Trim();

            try
            {
                using (MySqlConnection conn = Conexao.Abrir())
                {
                    // Verifica se já existe
                    using (MySqlCommand cmdVerifica = new MySqlCommand(
                        "SELECT COUNT(*) FROM usuario WHERE login = @login", conn))
                    {
                        cmdVerifica.Parameters.AddWithValue("@login", loginDigitado);
                        long quantidade = Convert.ToInt64(cmdVerifica.ExecuteScalar());

                        if (quantidade > 0)
                        {
                            Log.Info("Tentativa de cadastro com login já existente: " + loginDigitado);

                            MessageBox.Show("Esse usuário já existe. Escolha outro.", "Atenção",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtLogin.Focus();
                            txtLogin.SelectAll();
                            return;
                        }
                    }

                    // Cadastra com hash
                    using (MySqlCommand cmdInsert = new MySqlCommand(
                        "INSERT INTO usuario (login, senha, perfil) VALUES (@login, @senha, 'Operador')", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@login", loginDigitado);
                        cmdInsert.Parameters.AddWithValue("@senha", Security.GerarHash(txtSenha.Text));
                        cmdInsert.ExecuteNonQuery();
                    }
                }

                Log.Info("Novo usuário cadastrado: " + loginDigitado);

                MessageBox.Show(
                    "Conta criada com sucesso! 🎉\n\n" +
                    "Agora é só voltar e fazer login.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.Close();
            }
            catch (MySqlException ex)
            {
                Log.Erro("Erro ao cadastrar usuário", ex);
                MessageBox.Show("Erro ao cadastrar no banco:\n\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Log.Erro("Erro inesperado no cadastro", ex);
                MessageBox.Show("Erro inesperado:\n\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}