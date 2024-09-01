using Gdk;
using Gtk;

namespace CryptoTrackApp.src.view.Components
{
    public class CryptoTreeViewComponent : ScrolledWindow
    {
        private ListStore listStore;
        // private TreeView treeView;

        public CryptoTreeViewComponent()
        {
            // Configurar las políticas de desplazamiento
            this.SetPolicy(PolicyType.Never, PolicyType.Automatic);

            // Inicializar el ListStore
            listStore = new ListStore(typeof(Pixbuf), typeof(string), typeof(int), typeof(double), typeof(float));

            // Inicializar el TreeView con el ListStore
            var treeView = new TreeView(listStore);

            // Configurar las columnas del TreeView
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

            // Estilizar columnas y celdas
            foreach (TreeViewColumn item in new TreeViewColumn[]{nameColumn, rankColumn, usdColumn, tendencyColumn})
            {
                item.Alignment = 0.50F;
                item.MinWidth = 200;
            }

            foreach (CellRendererText item in new CellRendererText[]{nameRenderer, rankRenderer, usdPriceRenderer, tendencyRenderer})
            {
                item.FontDesc = Pango.FontDescription.FromString("Arimo italic 12");
            }

            // Añadir las columnas al TreeView
            treeView.AppendColumn(iconColumn);
            treeView.AppendColumn(nameColumn);
            treeView.AppendColumn(rankColumn);
            treeView.AppendColumn(usdColumn);
            treeView.AppendColumn(tendencyColumn);

            treeView.Hexpand = true;
            treeView.Vexpand = true;
            // Asegurarse de que el TreeView se expanda completamente
            treeView.Hexpand = true;
            treeView.Vexpand = true;

            // Añadir el TreeView al ScrolledWindow
            this.Add(treeView);
            /* Viewport viewport = new Viewport();
            viewport.Add(treeView);

            viewport.SetSizeRequest(500, 200);
            this.Add(viewport);
 */

            // Asegurarse de que el ScrolledWindow return treeView;se expanda completamente
            this.Hexpand = true;
            this.Vexpand = true;
            this.SetSizeRequest(600, 400);
        }

        public void AddData(Pixbuf icon, string name, int rank, double usdPrice, float tendency)
        { 
            // Agregar nuevos datos al ListStore
            listStore.AppendValues(icon, name, rank, usdPrice, tendency);
        }
    }
}
