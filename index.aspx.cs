using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace logtest
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuariologueado"] != null)
            {
                string usuariologueado = Session["usuariologueado"].ToString();
                lblBienvenida.Text = "Bienvenido/a " + usuariologueado;
            }
            else
            {
                Response.Redirect("login.aspx");
            }

            lblError.Text = "";
            lblErrorContrasenia.Text = "";
            LeerDatos();
        }
        protected void BtnCerrar_Click(object sender, EventArgs e)
        {
            Session.Remove("usuariologueado");
            Response.Redirect("login.aspx");
        }
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);

        void Limpiar()
        {
            tbNombres.Text = "";
            tbApellidos.Text = "";
            tbFechaNacimiento.Text = "";
            tbUsuario.Text = "";
            tbConfirmarContrasenia.Text = "";
            tbContrasenia.Text = "";
            lblError.Text = "";
            lblErrorContrasenia.Text = "";
        }
        protected void BtnRegistrar_Click(Object sender, EventArgs e)
        {
            if (tbNombres.Text == "" || tbApellidos.Text == "" || tbFechaNacimiento.Text == "" || tbUsuario.Text == "" || tbConfirmarContrasenia.Text == "" || tbContrasenia.Text == "")
            {
                lblError.Text = "Ningun campo puede quedar vacío!";
            }
            else
            {
                if (tbContrasenia.Text != tbConfirmarContrasenia.Text)
                {
                    lblErrorContrasenia.Text = "Las contrasenias no coinciden!";
                }
                else
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("select count(*) from Usuarios where Usuario='" + tbUsuario.Text + "'", conexion)
                    {
                        CommandType = System.Data.CommandType.Text
                    };
                    cmd.Parameters.AddWithValue("Usuario", tbUsuario.Text);
                    int usuario = Convert.ToInt32(cmd.ExecuteScalar());
                    string patron = "xxsebasxx";
                    if (usuario < 1)
                    {
                        SqlCommand cmm = new SqlCommand("Insert into Usuarios values('" + tbNombres.Text + "','" + tbApellidos.Text + "'," +
                            "'" + tbFechaNacimiento.Text + "','" + tbUsuario.Text + "',(EncryptByPassPhrase('" + patron + "','" + tbContrasenia.Text + "')))", conexion);
                        cmm.ExecuteNonQuery();
                        conexion.Close();
                        Limpiar();
                        LeerDatos();
                    }
                    else
                    {
                        lblError.Text = "El Usuario " + tbUsuario.Text + " ya existe!";
                        tbUsuario.Text = "";
                    }
                }
            }
        }

        void LeerDatos()
        {
            SqlCommand leerdatos = new SqlCommand("Select * from Usuarios", conexion);
            SqlDataAdapter da = new SqlDataAdapter(leerdatos);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gvUsuarios.DataSource = dt;
            gvUsuarios.DataBind();
        }

    }
}
