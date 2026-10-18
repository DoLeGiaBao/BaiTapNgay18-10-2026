using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bai4_12;
public class MemberModel
{
    public string MemberId { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Package { get; set; } = "";
    public DateTime StartDate { get; set; }
    public decimal Fee { get; set; }
}
public class Form1 : Form
{
    BindingList<MemberModel> members = new();
    BindingSource bs = new();
    ErrorProvider ep = new();
    TextBox id = new(), nameBox = new(), phone = new(), fee = new();
    ComboBox packageBox = new();
    DateTimePicker start = new();
    DataGridView grid = new();
    MemberModel selected;
    public Form1()
    {
        Text = "Bài 4.12 - Gym Member Manager"; Width = 1050; Height = 650; MinimumSize = new Size(800, 500); StartPosition = FormStartPosition.CenterScreen;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 2, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); Controls.Add(root);
        var top = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 4, RowCount = 3 };
        for (int i=0;i<4;i++) top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25));
        AddText(top,"Mã hội viên",id,0,0,true); AddText(top,"Họ và tên",nameBox,1,0); AddText(top,"Số điện thoại",phone,2,0);
        var p = new Panel { Dock=DockStyle.Fill, Height=55 }; p.Controls.Add(new Label{Text="Gói tập",Dock=DockStyle.Top}); packageBox.Dock=DockStyle.Top; packageBox.DropDownStyle=ComboBoxStyle.DropDownList; packageBox.Items.AddRange(new object[]{"Theo tháng","6 tháng","1 năm"}); packageBox.SelectedIndex=0; p.Controls.Add(packageBox); top.Controls.Add(p,3,0);
        var dp = new Panel{Dock=DockStyle.Fill,Height=55}; dp.Controls.Add(new Label{Text="Ngày đăng ký",Dock=DockStyle.Top}); start.Dock=DockStyle.Top; start.Format=DateTimePickerFormat.Short; dp.Controls.Add(start); top.Controls.Add(dp,0,1);
        AddText(top,"Phí dịch vụ",fee,1,1);
        var actions = new FlowLayoutPanel{Dock=DockStyle.Fill,AutoSize=true};
        Button add = Btn("Thêm mới");
        Button edit = Btn("Sửa thông tin");
        Button del = Btn("Xóa hội viên");
        Button clear = Btn("Làm mới");
        actions.Controls.AddRange(new Control[]{add,edit,del,clear}); top.Controls.Add(actions,2,1); top.SetColumnSpan(actions,2);
        root.Controls.Add(top,0,0);
        grid.Dock=DockStyle.Fill; grid.ReadOnly=true; grid.AutoGenerateColumns=false; grid.AllowUserToAddRows=false; grid.SelectionMode=DataGridViewSelectionMode.FullRowSelect; grid.MultiSelect=false;
        Col("Mã HV","MemberId",90); Col("Tên hội viên","FullName",180); Col("SĐT","Phone",120); Col("Gói tập","Package",110); Col("Ngày ĐK","StartDate",110,"dd/MM/yyyy"); Col("Phí dịch vụ","Fee",120,"N0");
        root.Controls.Add(grid,0,1); bs.DataSource=members; grid.DataSource=bs; ep.ContainerControl=this;
        members.Add(new(){MemberId="HV001",FullName="Nguyễn Minh Anh",Phone="0912345678",Package="Theo tháng",StartDate=DateTime.Today,Fee=500000});
        members.Add(new(){MemberId="HV002",FullName="Trần Quốc Bảo",Phone="0987654321",Package="6 tháng",StartDate=DateTime.Today.AddDays(-5),Fee=2700000});
        grid.SelectionChanged += (_,_)=>LoadRow();
        add.Click+=(_,_)=>AddMember(); edit.Click+=(_,_)=>EditMember(); del.Click+=(_,_)=>DeleteMember(); clear.Click+=(_,_)=>ClearForm();
        ClearForm();
    }
    void AddText(TableLayoutPanel t,string label,TextBox b,int c,int r,bool ro=false){var p=new Panel{Dock=DockStyle.Fill,Height=55};p.Controls.Add(new Label{Text=label,Dock=DockStyle.Top});b.Dock=DockStyle.Top;b.ReadOnly=ro;p.Controls.Add(b);t.Controls.Add(p,c,r);}
    Button Btn(string s)=>new(){Text=s,AutoSize=true};
    void Col(string h,string p,int w,string f=null)=>grid.Columns.Add(new DataGridViewTextBoxColumn{HeaderText=h,DataPropertyName=p,Width=w,DefaultCellStyle=new DataGridViewCellStyle{Format=f??""}});
    string NextId()=>"HV"+(members.Select(x=>int.TryParse(x.MemberId.Replace("HV",""),out int n)?n:0).DefaultIfEmpty().Max()+1).ToString("000");
    bool Valid(){ep.Clear();bool ok=true;if(!Regex.IsMatch(nameBox.Text.Trim(),@"^[\p{L}\s'-]+$")){ep.SetError(nameBox,"Nhập họ tên hợp lệ.");ok=false;}if(!Regex.IsMatch(phone.Text.Trim(),@"^\d{10}$")){ep.SetError(phone,"SĐT phải đúng 10 chữ số.");ok=false;}if(!decimal.TryParse(fee.Text,NumberStyles.Number,CultureInfo.CurrentCulture,out var f)||f<=0||f!=decimal.Truncate(f)){ep.SetError(fee,"Phí phải là số nguyên dương.");ok=false;}if(members.Any(x=>x.MemberId==id.Text && !ReferenceEquals(x,selected))){ep.SetError(id,"Mã hội viên trùng.");ok=false;}return ok;}
    MemberModel Read()=>new(){MemberId=id.Text,FullName=nameBox.Text.Trim(),Phone=phone.Text.Trim(),Package=packageBox.Text,StartDate=start.Value.Date,Fee=decimal.Parse(fee.Text,NumberStyles.Number,CultureInfo.CurrentCulture)};
    void AddMember(){selected=null;if(!Valid())return;members.Add(Read());ClearForm();}
    void EditMember(){if(selected==null){MessageBox.Show("Chọn hội viên cần sửa.");return;}if(!Valid())return;var x=Read();selected.FullName=x.FullName;selected.Phone=x.Phone;selected.Package=x.Package;selected.StartDate=x.StartDate;selected.Fee=x.Fee;bs.ResetBindings(false);ClearForm();}
    void DeleteMember(){if(selected==null)return;if(MessageBox.Show("Xóa hội viên này?","Xác nhận",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)members.Remove(selected);ClearForm();}
    void LoadRow(){if(grid.CurrentRow?.DataBoundItem is not MemberModel m)return;selected=m;id.Text=m.MemberId;nameBox.Text=m.FullName;phone.Text=m.Phone;packageBox.SelectedItem=m.Package;start.Value=m.StartDate;fee.Text=m.Fee.ToString("0",CultureInfo.CurrentCulture);}
    void ClearForm(){selected=null;ep.Clear();id.Text=NextId();nameBox.Clear();phone.Clear();fee.Clear();packageBox.SelectedIndex=0;start.Value=DateTime.Today;grid.ClearSelection();}
}
