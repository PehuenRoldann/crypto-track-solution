using Gdk;
using Gtk;
using System;

namespace CryptoTrackApp.src.view.Components
{
    public class CryptoTreeViewComponent : ScrolledWindow
    {
        private ListStore listStore;
         // Delegado para manejar el evento de fila activada con la información de la fila
        public event EventHandler<CryptoRowActivatedEventArgs> RowActivatedEvent;

        public CryptoTreeViewComponent(int width = 100, int heigh = 500)
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
                item.MinWidth = 50;
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

            // Conectar el evento RowActivated
            treeView.RowActivated += OnRowActivated;

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
            this.Valign = Align.Center;
            this.SetSizeRequest(width, heigh);
        }

        public void AddData(Pixbuf icon, string name, int rank, double usdPrice, float tendency)
        { 
            // Agregar nuevos datos al ListStore
            listStore.AppendValues(icon, name, rank, usdPrice, tendency);
        }

        private void OnRowActivated(object sender, RowActivatedArgs args)
        {
            // Recuperar la información de la fila activada
            if (listStore.GetIter(out TreeIter iter, args.Path))
            {
                string name = (string)listStore.GetValue(iter, 1);
                int rank = (int)listStore.GetValue(iter, 2);
                double usdPrice = (double)listStore.GetValue(iter, 3);
                float tendency = (float)listStore.GetValue(iter, 4);

                Console.WriteLine($"Recuperada línea con name: {name}");
                // Crear un objeto personalizado con los datos de la fila activada
                var eventArgs = new CryptoRowActivatedEventArgs( name, rank, usdPrice, tendency);

                // Propagar el evento al contenedor padre con la información de la fila activada
                RowActivatedEvent?.Invoke(this, eventArgs);
            }
        }
    }

    // Clase para los argumentos del evento personalizado
    public class CryptoRowActivatedEventArgs : EventArgs
    {
        public Pixbuf Icon { get; }
        public string Name { get; }
        public int Rank { get; }
        public double UsdPrice { get; }
        public float Tendency { get; }

        public CryptoRowActivatedEventArgs(string name, int rank, double usdPrice, float tendency)
        {
            Name = name;
            Rank = rank;
            UsdPrice = usdPrice;
            Tendency = tendency;
        }
    }
}

