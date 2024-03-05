using Gdk;
using Gtk;

namespace CryptoTrackApp.src.view.Components
{
    public class CryptoTreeViewComponent : AspectFrame
    {
        private ListStore listStore;
        private TreeView treeView;

        public CryptoTreeViewComponent() : base ("", 0, 0, 10, true)
        {

            listStore = new ListStore(typeof(Pixbuf), typeof(string), typeof(int), typeof(double), typeof(float));

            /* listStore.AppendValues(Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.currency.btc.png"),
            "bitcoin", 1, 19332.2, 0.4);
            listStore.AppendValues(Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.currency.btc.png"),
            "dogecoin", 3, 193.54, 0.1);
            listStore.AppendValues(Pixbuf.LoadFromResource("CryptoTrackApp.src.assets.icons.currency.dai.png"),
            "dai", 145, 1, 0.3);  */
        
            treeView = new TreeView(listStore);

            var iconRenderer = new CellRendererPixbuf();
            var nameRenderer = new CellRendererText();
            var rankRenderer = new CellRendererText();
            var usdPriceRenderer = new CellRendererText();
            var tendencyRenderer = new CellRendererText();

            TreeViewColumn iconColumn = new TreeViewColumn("Icon", iconRenderer);
            iconColumn.AddAttribute(iconRenderer, "pixbuf", 0);
            TreeViewColumn nameColumn = new TreeViewColumn("Name", nameRenderer, "text", 1);
            TreeViewColumn rankColumn = new TreeViewColumn("Rank", rankRenderer, "text", 2);
            TreeViewColumn usdColumn = new TreeViewColumn("$USD", usdPriceRenderer, "text", 3);
            TreeViewColumn tendencyColumn = new TreeViewColumn("Tendency", tendencyRenderer, "text", 4);

            foreach (TreeViewColumn item in new TreeViewColumn[]{nameColumn, rankColumn,
            usdColumn, tendencyColumn})
            {
                item.Alignment = 0.50F;
                item.MinWidth = 200;
            }

            foreach (CellRendererText item in new CellRendererText[]{nameRenderer, 
            rankRenderer, usdPriceRenderer, tendencyRenderer})
            {
                item.FontDesc = Pango.FontDescription.FromString("Arimo italic 12");
            }

            treeView.AppendColumn(iconColumn);
            treeView.AppendColumn(nameColumn);
            treeView.AppendColumn(rankColumn);
            treeView.AppendColumn(usdColumn);
            treeView.AppendColumn(tendencyColumn);

            this.Add(treeView);

        }

        public void AddData(Pixbuf icon, string name, int rank, double usdPrice, float tendency)
        { 
            // Agregar nuevos datos al ListStore
            listStore.AppendValues(icon, name, rank, usdPrice, tendency);
        }
        
    }
}