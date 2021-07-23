using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XBCAD_Stock_Taking_application
{
    public partial class View_Stock_Finance : Form
    {
        private ArrayList brandNames = new ArrayList();
        private ArrayList catergoryNames = new ArrayList();
        private ArrayList subCatergoryNames = new ArrayList();
        private int test;
        
        public View_Stock_Finance()
        {
            InitializeComponent();
            setUpAltInfo();                       
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindGridView();
            GridViewDimensions();
            populateInsert();
        }

        public void BindGridView()
        {
            String query = "Select *, Unit*Price_Each AS Price_Total from [Stock_Main]";

          SqlConnection con = new SqlConnection();
          con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();        
          SqlCommand cmd = new SqlCommand(query,con);
            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        public void GridViewDimensions()
        {
            dataGridView1.Columns[0].Width = 100;
            dataGridView1.Columns[1].Width = 200;
            dataGridView1.Columns[2].Width = 275;
            dataGridView1.Columns[3].Width = 75;
            dataGridView1.Columns[4].Width = 40;
            dataGridView1.Columns[7].Width = 200;
            dataGridView1.Columns[8].Width = 150;
        }

        private void setUpAltInfo()
        {
            gbLoadCellData.Visible = false;
            gbLoadCellData.Left = 10;
            gbLoadCellData.Top = 233;
            gbLoadCellData.Height = 173;

            gbMiscData.Visible = false;
            gbMiscData.Left = 10;
            gbMiscData.Top = 233;
            gbMiscData.Height = 173;

            gbScaleData.Visible = false;
            gbScaleData.Left = 10;
            gbScaleData.Top = 233;
            gbScaleData.Height = 173;

            gbWeightData.Visible = false;
            gbWeightData.Left = 10;
            gbWeightData.Top = 233;
            gbWeightData.Height = 173;

            gbWeightSetData.Visible = false;
            gbWeightSetData.Left = 10;
            gbWeightSetData.Top = 233;
            gbWeightSetData.Height = 173;
        }

        private void cbGenTypeID_SelectedIndexChanged(object sender, EventArgs e)
        {
            String typeID = cbGenTypeID.Text;

            gbLoadCellData.Visible = false;
            gbMiscData.Visible = false;
            gbScaleData.Visible = false;
            gbWeightData.Visible = false;
            gbWeightSetData.Visible = false;

            if (typeID.Equals("Scale"))
            {
                gbScaleData.Visible = true;
            }
            else if (typeID.Equals("Loadcell"))
            {
                gbLoadCellData.Visible = true;
            }
            else if (typeID.Equals("Weight"))
            {
                gbWeightData.Visible = true;
            }
            else if (typeID.Equals("Weight_Set"))
            {
                gbWeightSetData.Visible = true;
            }
            else if (typeID.Equals("Misc"))
            {
                gbMiscData.Visible = true;
            }
        }

        private void populateInsert()
        {
            String query = "Select Brand, Catergory, Sub_Catergory from [Stock_Main]";//Catergory, Sub_Catergory

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            //ArrayList brandNames = new ArrayList();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        String temp = reader.GetString(0);
                        if (!brandNames.Contains(temp))
                        {
                            brandNames.Add(temp);
                        }
                    }
                    catch (Exception)
                    {}

                    try
                    {
                        String temp = reader.GetString(1);
                        if (!catergoryNames.Contains(temp))
                        {
                            catergoryNames.Add(temp);
                        }
                    }
                    catch (Exception)
                    { }

                    try
                    {
                        String temp = reader.GetString(2);
                        if (!subCatergoryNames.Contains(temp))
                        {
                            subCatergoryNames.Add(temp);
                        }
                    }
                    catch (Exception)
                    { }

                }
            }
            foreach (String item in brandNames)
            {
                cbGenBrand.Items.Add(item);
                cbBrand.Items.Add(item);
            }

            foreach (String item in catergoryNames)
            {
                cbGenCatagory.Items.Add(item);
                cbCatergories.Items.Add(item);
            }

            foreach (String item in subCatergoryNames)
            {
                cbGenSubCat.Items.Add(item);
                cbSubCatergories.Items.Add(item);
            }

            con.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txtGenTitle.Text.Length == 0)
            {
                MessageBox.Show("Please fill out all required fields");
            }
            else if (cbGenCatagory.Text.Length == 0)
            {
                MessageBox.Show("Please fill out all required fields");
            }
            else if (cbGenTypeID.Text.Length == 0)
            {
                MessageBox.Show("Please fill out all required fields");
            }
            else
            {
                if (cbGenTypeID.SelectedIndex == 0)
                {
                    int id = insertGen("Scale");
                    insertScale(id);
                }
                else if (cbGenTypeID.SelectedIndex == 1)
                {
                    int id = insertGen("Loadcell");
                    insertLoadCell(id);
                }
                else if(cbGenTypeID.SelectedIndex == 2)
                {
                    int id = insertGen("Weight");
                    insertWeight(id);
                }
                else if(cbGenTypeID.SelectedIndex == 3)
                {
                    if (txtWSMaxMass.Value>txtWSMinMass.Value)
                    {
                        int id = insertGen("Weight_Sets");
                        insertWeightSet(id);
                    }
                    else
                    {
                        MessageBox.Show("Minimum Value cannot be more than Maximum");
                    }
                }
                else if(cbGenTypeID.SelectedIndex == 4)
                {
                    int id = insertGen("Misc");
                    insertMisc(id);
                }
             }
            BindGridView();
        }

        private int insertGen(String typeID) {

            string insertStockMain = "Insert into [Stock_Main](Brand, Title, Date, Unit, Price_Each, Type_ID, Catergory, Sub_Catergory)" +
                                          "Values(@valBrand, @valTitle, @valDate, @valUnit, @valPrice_Each, @valType_ID, @valCatergory, @valSub_Catergory)";

            decimal priceEach = txtGenPrice.Value;
            decimal unit = txtGenUnit.Value;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(insertStockMain, con);
            con.Open();

            cmd.Parameters.AddWithValue("valBrand", cbGenBrand.Text);
            cmd.Parameters.AddWithValue("valTitle", txtGenTitle.Text);
            if (rbtnGenNoDate.Checked)
            {
                cmd.Parameters.AddWithValue("valDate", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("valDate", dtGenDate.Value);
            }
            if (unit == 0)
            {
                cmd.Parameters.AddWithValue("valUnit", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("valUnit", unit);
            }
            if (priceEach == 0)
            {
                cmd.Parameters.AddWithValue("valPrice_Each", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("valPrice_Each", priceEach);
            }            
            cmd.Parameters.AddWithValue("valType_ID", typeID);
            cmd.Parameters.AddWithValue("valCatergory", cbGenCatagory.SelectedItem.ToString());
            cmd.Parameters.AddWithValue("valSub_Catergory", cbGenSubCat.Text);

            int result1 = cmd.ExecuteNonQuery();
            con.Close();

            int totalEntries = 0;
            String query = "Select count(*) from [Stock_Main]";
            SqlCommand cmd3 = new SqlCommand(query, con);
            con.Open();

            using (SqlDataReader reader = cmd3.ExecuteReader())
            {
                while (reader.Read())
                {
                    totalEntries = reader.GetInt32(0);
                }
            }
            con.Close();
            totalEntries--;


            int id = -1;
            String query2 = "Select Stock_ID from [Stock_Main] where Stock_ID not in (" +
                "select top " + totalEntries + " Stock_ID from Stock_ID)";           
            SqlCommand cmd4 = new SqlCommand(query2, con);
            con.Open();

            using (SqlDataReader reader = cmd3.ExecuteReader())
            {
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                }
            }
            con.Close();
            return id;
        }

        private void insertScale(int id) 
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();

            string dimensionLength = txtScaleDimL.Value.ToString();
            string dimensionLength2 = dimensionLength + " x ";

            bool isWaterProof = rbtnScaleWaterproof.Checked;

            string insertScale = "Insert into [Scales](S_Stock_ID,Limit, Limit_Unit, Dimension_Length, Dimension_Width, Is_Water_Proof)" +
                                 "Values(@valID, @valLimit, @valLimit_Unit, @valDimension_Length, @valDimension_Width, @valIs_Water_Proof)"; //Dimension unit has not been included.

            SqlCommand cmd = new SqlCommand(insertScale, con);
            con.Open();

            cmd.Parameters.AddWithValue("valID", id);
            cmd.Parameters.AddWithValue("valLimit", txtScaleLimit.Value);
            cmd.Parameters.AddWithValue("valLimit_Unit", cbScaleLimitUnit.Text);
            cmd.Parameters.AddWithValue("valDimension_Length", dimensionLength2);
            cmd.Parameters.AddWithValue("valDimension_Width", txtScaleDimW.Value.ToString());
            cmd.Parameters.AddWithValue("valIs_Water_Proof", isWaterProof);

            int result2 = cmd.ExecuteNonQuery();

            if ( result2 > 0)
            {
                MessageBox.Show("Item has been added.");
            }
            con.Close();
        }
        private void insertLoadCell(int id) 
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();

            string insertLoadcell = "Insert into [Loadcells](L_Stock_ID, L_Mass, L_Mass_Unit)" +
                                 "Values(@valID, @valL_Mass, @valL_Mass_Unit)"; 

            SqlCommand cmd = new SqlCommand(insertLoadcell, con);
            con.Open();

            cmd.Parameters.AddWithValue("valID", id);
            cmd.Parameters.AddWithValue("valL_Mass", txtLoadMass.Value);
            cmd.Parameters.AddWithValue("valL_Mass_Unit", cbLoadMU.Text);

            int result2 = cmd.ExecuteNonQuery();

            if (result2 > 0)
            {
                MessageBox.Show("Item has been added.");
            }
            con.Close();
        }
        private void insertWeight(int id) 
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();

            string insertWeights = "Insert into [Weights](W_Stock_ID, W_Mass, W_Mass_Unit)" +
                                 "Values(@valID, @valW_Mass, @valW_Mass_Unit)";

            SqlCommand cmd = new SqlCommand(insertWeights, con);
            con.Open();

            cmd.Parameters.AddWithValue("valID", id);
            cmd.Parameters.AddWithValue("valW_Mass", txtWeightMass.Value);
            cmd.Parameters.AddWithValue("valW_Mass_Unit", cbWeightMU.Text);

            int result2 = cmd.ExecuteNonQuery();

            if (result2 > 0)
            {
                MessageBox.Show("Item has been added.");
            }
            con.Close();
        }
        private void insertWeightSet(int id) 
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();

            string insertWeightSet = "Insert into [Weight_Sets](WS_Stock_ID, MinMass, MaxMass, WS_Min_Mass_Unit, WS_Max_Mass_Unit)" +
                                 "Values(@valID, @valMin_Mass, @valMax_Mass, @valMin_Mass_Unit, @valMax_Mass_Unit)";

            SqlCommand cmd = new SqlCommand(insertWeightSet, con);
            con.Open();

            cmd.Parameters.AddWithValue("valID", id);
            cmd.Parameters.AddWithValue("valMin_Mass", txtWSMinMass.Value);
            cmd.Parameters.AddWithValue("valMax_Mass", txtWSMaxMass.Value);
            cmd.Parameters.AddWithValue("valMin_Mass_Unit", cbWSMinMassU.Text + " - ");
            cmd.Parameters.AddWithValue("valMax_Mass_Unit", cbWSMaxMassU.Text);

            int result2 = cmd.ExecuteNonQuery();

            if (result2 > 0)
            {
                MessageBox.Show("Item has been added.");
            }
            con.Close();
        }
        private void insertMisc(int id) 
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();

            string insertWeightSet = "Insert into [Misc](M_Stock_ID, Added_Info)" +
                                 "Values(@valID, @Added_Info)";

            SqlCommand cmd = new SqlCommand(insertWeightSet, con);
            con.Open();

            cmd.Parameters.AddWithValue("valID", id);
            cmd.Parameters.AddWithValue("Added_Info", txtMiscAddedInfo.Text);
           
            int result2 = cmd.ExecuteNonQuery();

            if (result2 > 0)
            {
                MessageBox.Show("Item has been added.");
            }
            con.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            BindGridView();
        }

        private void cbTypeID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedType = cbTypeID.SelectedIndex;
            if (selectedType==0)
            {
                getScales();
            }
            else if (selectedType == 1)
            {
                getLoadCell();
            }
            else if (selectedType == 2)
            {
                getWeight();
            }
            else if (selectedType == 3)
            {
                getWeightSet();
            }
            else if (selectedType == 4)
            {
                getMisc();
            }
        }

        private void getScales() {

            String query = "Select Stock_Main.Stock_ID, Stock_Main.Brand, Stock_Main.Title, Stock_Main.Date, Stock_Main.Unit, Stock_Main.Price_Each, Stock_Main.Unit*Stock_Main.Price_Each AS Price_Total, Stock_Main.Type_ID, Stock_Main.Catergory , Stock_Main.Sub_Catergory, Scales.Limit, Scales.Limit_Unit, CONCAT(Scales.Dimension_Length, '', Scales.Dimension_Width) AS Dimensions, is_Water_Proof" +
                           " From [Stock_Main],[Scales] " +
                           "WHERE Stock_Main.Stock_ID = Scales.S_Stock_ID";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
           
            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }
        private void getLoadCell() 
        {
            String query = "Select Stock_Main.Stock_ID, Stock_Main.Brand, Stock_Main.Title, Stock_Main.Date, Stock_Main.Unit, Stock_Main.Price_Each, Stock_Main.Unit*Stock_Main.Price_Each AS Price_Total, Stock_Main.Type_ID, Stock_Main.Catergory, Stock_Main.Sub_Catergory, Loadcells.L_Mass, Loadcells.L_Mass_Unit" +
                           " From [Stock_Main],[Loadcells] " +
                           "WHERE Stock_Main.Stock_ID = Loadcells.L_Stock_ID";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();

        }
        private void getWeight() 
        {
            String query = "Select Stock_Main.Stock_ID, Stock_Main.Brand, Stock_Main.Title, Stock_Main.Date, Stock_Main.Unit, Stock_Main.Price_Each, Stock_Main.Unit*Stock_Main.Price_Each AS Price_Total, Stock_Main.Type_ID, Stock_Main.Catergory, Stock_Main.Sub_Catergory, Weights.W_Mass, Weights.W_Mass_Unit" +
                           " From [Stock_Main],[Weights] " +
                           "WHERE Stock_Main.Stock_ID = Weights.W_Stock_ID";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }
        private void getWeightSet() 
        {
            String query = "Select Stock_Main.Stock_ID, Stock_Main.Brand, Stock_Main.Title, Stock_Main.Date, Stock_Main.Unit, Stock_Main.Price_Each, Stock_Main.Unit*Stock_Main.Price_Each AS Price_Total, Stock_Main.Type_ID, Stock_Main.Catergory, Stock_Main.Sub_Catergory, CONCAT(Weight_Sets.MinMass, '', Weight_Sets.WS_Min_Mass_Unit, '', Weight_Sets.MinMass, '', Weight_Sets.WS_Max_Mass_Unit) AS [Set Range]" +
                           " From [Stock_Main],[Weight_Sets] " +
                           "WHERE Stock_Main.Stock_ID = Weight_Sets.WS_Stock_ID";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }
        private void getMisc() 
        {
            String query = "Select Stock_Main.Stock_ID, Stock_Main.Brand, Stock_Main.Title, Stock_Main.Date, Stock_Main.Unit, Stock_Main.Price_Each, Stock_Main.Unit*Stock_Main.Price_Each AS Price_Total, Stock_Main.Type_ID, Stock_Main.Catergory, Stock_Main.Sub_Catergory, Misc.Added_Info" +
                           " From [Stock_Main],[Misc] " +
                           "WHERE Stock_Main.Stock_ID = Misc.M_Stock_ID";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        private void cbCatergories_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCat = cbCatergories.SelectedItem.ToString();
            getSelectedCatergory(selectedCat);
        }
        private void getSelectedCatergory(string selectedCat)
        {
            String query = "Select Stock_Main.Stock_ID, Stock_Main.Brand, Stock_Main.Title, Stock_Main.Date, Stock_Main.Unit, Stock_Main.Price_Each, Stock_Main.Unit*Stock_Main.Price_Each AS Price_Total, Stock_Main.Type_ID, Stock_Main.Catergory , Stock_Main.Sub_Catergory" +
                           " From [Stock_Main] " +
                           "Where Stock_Main.Catergory Like @valCatergories";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@valCatergories", selectedCat);
            con.Open();

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        
        private void cbSubCatergories_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSubCat = cbSubCatergories.SelectedItem.ToString();
            getSelectedSubCatergory(selectedSubCat);
        }
        private void getSelectedSubCatergory(string selectedSubCat)
        {
            String query = "Select Stock_Main.Stock_ID, Stock_Main.Brand, Stock_Main.Title, Stock_Main.Date, Stock_Main.Unit, Stock_Main.Price_Each, Stock_Main.Unit*Stock_Main.Price_Each AS Price_Total, Stock_Main.Type_ID, Stock_Main.Catergory , Stock_Main.Sub_Catergory" +
                           " From [Stock_Main] " +
                           "Where Stock_Main.Sub_Catergory Like @valSubCatergory";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@valSubCatergory", selectedSubCat);
            con.Open();

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }


        private void cbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBrand = cbBrand.SelectedItem.ToString();
            getSelectedBrand(selectedBrand);
        }
        private void getSelectedBrand(string selectedBrand)
        {
            String query = "Select Stock_Main.Stock_ID, Stock_Main.Brand, Stock_Main.Title, Stock_Main.Date, Stock_Main.Unit, Stock_Main.Price_Each, Stock_Main.Unit*Stock_Main.Price_Each AS Price_Total, Stock_Main.Type_ID, Stock_Main.Catergory , Stock_Main.Sub_Catergory" +
                           " From [Stock_Main] " +
                           "Where Stock_Main.Brand Like @valBrand";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@valBrand", selectedBrand);
            con.Open();

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }
    }
}
