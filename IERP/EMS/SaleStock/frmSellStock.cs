using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EMS.SaleStock
{
    public partial class frmSellStock : Form
    {
        BaseClass.BaseInfo baseinfo = new EMS.BaseClass.BaseInfo();
        BaseClass.cBillInfo billinfo = new EMS.BaseClass.cBillInfo();
        BaseClass.cCurrentAccount currentAccount = new EMS.BaseClass.cCurrentAccount();
        BaseClass.cStockInfo stockinfo = new EMS.BaseClass.cStockInfo();
        public frmSellStock()
        {
            InitializeComponent();
        }

        private void frmSellStock_Load(object sender, EventArgs e)
        {
            txtBillDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            DataSet ds = null;
            string P_Str_newBillCode = "";
            int P_Int_newBillCode = 0;

            ds = baseinfo.GetAllBill("tb_sell_main");
            if (ds.Tables[0].Rows.Count == 0)
            {
                txtBillCode.Text = DateTime.Now.ToString("yyyyMMdd") + "XS" + "1000001";
            }
            else
            {
                P_Str_newBillCode = Convert.ToString(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["billcode"]);
                P_Int_newBillCode = Convert.ToInt32(P_Str_newBillCode.Substring(10, 7)) + 1;
                P_Str_newBillCode = DateTime.Now.ToString("yyyyMMdd") + "XS" + P_Int_newBillCode.ToString();
                txtBillCode.Text = P_Str_newBillCode;
            }
            txtHandle.Focus();
        }

        private void btnSelectHandle_Click(object sender, EventArgs e)
        {
            EMS.SelectDataDialog.frmSelectHandle selecthandle;
            selecthandle = new EMS.SelectDataDialog.frmSelectHandle();
            selecthandle.sellStock = this;          //���´����Ĵ����������Ϊͬһ���������ʵ��������
            selecthandle.M_str_object = "SellStock";����//����ʶ������һ��������õ�selecthandle���ڵ�
            selecthandle.ShowDialog();
        }

        private void btnSelectUnits_Click(object sender, EventArgs e)
        {
            EMS.SelectDataDialog.frmSelectUnits selectUnits;
            selectUnits = new EMS.SelectDataDialog.frmSelectUnits();
            selectUnits.sellStock = this;          //���´����Ĵ����������Ϊͬһ���������ʵ��������
            selectUnits.M_str_object = "SellStock";����//����ʶ������һ��������õ�selectUnits���ڵ�
            selectUnits.ShowDialog();
        }

        private void dgvStockList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectDataDialog.frmSelectStock selectStock = new EMS.SelectDataDialog.frmSelectStock();
            selectStock.sellStock = this;          //���´����Ĵ����������Ϊͬһ���������ʵ��������
            selectStock.M_int_CurrentRow = e.RowIndex;
            selectStock.M_str_object = "SellStock";����//����ʶ������һ��������õ�selectStock���ڵ�
            selectStock.ShowDialog();
        }

        private void dgvStockList_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            //ͳ����Ʒ���������ͽ��
            try
            {
                float tqty = 0;
                float tsum = 0;
                for (int i = 0; i <= dgvStockList.RowCount; i++)
                {
                    tsum = tsum + Convert.ToSingle(dgvStockList[5, i].Value.ToString());
                    tqty = tqty + Convert.ToSingle(dgvStockList[3, i].Value.ToString());
                    txtFullPayment.Text = tsum.ToString();
                    txtStockQty.Text = tqty.ToString();
                }

            }
            catch { }
        }

        private void dgvStockList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)����//���㣭��ͳ����Ʒ���
            {
                try
                {
                    float tsum = Convert.ToSingle(dgvStockList[3, e.RowIndex].Value.ToString()) * Convert.ToSingle(dgvStockList[4, e.RowIndex].Value.ToString());
                    dgvStockList[5, e.RowIndex].Value = tsum.ToString();
                }
                catch { }
            }
            if (e.ColumnIndex == 4)
            {
                try
                {
                    float tsum = Convert.ToSingle(dgvStockList[3, e.RowIndex].Value.ToString()) * Convert.ToSingle(dgvStockList[4, e.RowIndex].Value.ToString());
                    dgvStockList[5, e.RowIndex].Value = tsum.ToString();
                }
                catch { }
            }
        }

        private void txtpayment_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtBalance.Text = Convert.ToString(Convert.ToSingle(txtFullPayment.Text) - Convert.ToSingle(txtpayment.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("¼��Ƿ��ַ�������" + ex.Message, "������ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtpayment.Focus();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //������λ�;����˲���Ϊ�գ�
            if (txtHandle.Text == string.Empty || txtUnits.Text == string.Empty)
            {
                MessageBox.Show("������λ�;�����Ϊ�����", "������ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //�б������ݲ���Ϊ��
            if (Convert.ToString(dgvStockList[3, 0].Value) == string.Empty || Convert.ToString(dgvStockList[4, 0].Value) == string.Empty || Convert.ToString(dgvStockList[5, 0].Value) == string.Empty)
            {
                MessageBox.Show("���ʵ�б������ݣ����������������ۡ�����������Ϊ�գ�", "������ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Ӧ������Ϊ��
            if (txtFullPayment.Text.Trim() == "0")
            {
                MessageBox.Show("Ӧ������Ϊ��������", "������ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //�����۱���������¼����Ʒ������Ϣ
            billinfo.BillCode = txtBillCode.Text;
            billinfo.Handle = txtHandle.Text;
            billinfo.Units = txtUnits.Text;
            billinfo.Summary = txtSummary.Text;
            billinfo.FullPayment = Convert.ToSingle(txtFullPayment.Text);
            billinfo.Payment = Convert.ToSingle(txtpayment.Text);
            //ִ������
            baseinfo.AddTableMainSellhouse(billinfo, "tb_sell_main");

            //�����ۣ���ϸ������¼����Ʒ������Ϣ
            for (int i = 0; i < dgvStockList.RowCount - 1; i++)
            {
                billinfo.BillCode = txtBillCode.Text;
                billinfo.TradeCode = dgvStockList[0, i].Value.ToString();
                billinfo.FullName = dgvStockList[1, i].Value.ToString();
                billinfo.TradeUnit = dgvStockList[2, i].Value.ToString();
                billinfo.Qty = Convert.ToSingle(dgvStockList[3, i].Value.ToString());
                billinfo.Price = Convert.ToSingle(dgvStockList[4, i].Value.ToString());
                billinfo.TSum = Convert.ToSingle(dgvStockList[5, i].Value.ToString());
                //ִ�ж���¼�����ݣ����ӵ���ϸ���У�
                baseinfo.AddTableDetailedWarehouse(billinfo, "tb_sell_detailed");
                //���Ŀ������
                DataSet ds = null;
                stockinfo.TradeCode = dgvStockList[0, i].Value.ToString();
                ds = baseinfo.GetStockByTradeCode(stockinfo, "tb_Stock");
                stockinfo.Qty = Convert.ToSingle(ds.Tables[0].Rows[0]["qty"]);
                
                stockinfo.Qty = stockinfo.Qty - billinfo.Qty;
                int d = baseinfo.UpdateSaleStock_Qty(stockinfo);

            }
            //��������λ��ϸ��--¼������--��������Ϊ����
            currentAccount.BillCode = txtBillCode.Text;
            currentAccount.AddGathering = Convert.ToSingle(txtFullPayment.Text);
            currentAccount.FactAddFee = Convert.ToSingle(txtpayment.Text);
            currentAccount.Balance = Convert.ToSingle(txtBalance.Text);
            currentAccount.Units = txtUnits.Text;
            //ִ������
            int ca = baseinfo.AddCurrentAccount(currentAccount);
            MessageBox.Show("���۵��������˳ɹ���", "�ɹ���ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnEixt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}