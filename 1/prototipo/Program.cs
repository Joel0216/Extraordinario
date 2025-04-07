using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

class Productos
{
    public string nombreProducto { get; set; }
    public int CodigoProducto { get; set; }
    public int CantidadVenta { get; set; }
    public double PrecioUnitario { get; set; }

    public Productos(string nombreProducto, int CodigoProducto, int CantidadVenta, double PrecioUnitario)
    {
        this.nombreProducto = nombreProducto;
        this.CodigoProducto = CodigoProducto;
        this.CantidadVenta = CantidadVenta;
        this.PrecioUnitario = PrecioUnitario;
    }
}

class Registros
{
    private string _rutaArchivo;
    private List<Productos> _inventario;
    
    // Método público para obtener el inventario
    public List<Productos> ObtenerInventario()
    {
        return _inventario;
    }
    
    public List<Productos> Inventario => _inventario;

    public Registros(string _rutaArchivo)
    {
        this._rutaArchivo = _rutaArchivo;
        this._inventario = new List<Productos>();
        CargarInventario();
    }

    private void CargarInventario()
    {
        if (File.Exists(_rutaArchivo))
        {
            string[] lineas = File.ReadAllLines(_rutaArchivo);
            foreach (var linea in lineas)
            {
                string[] datos = linea.Split(',');
                if (datos.Length == 4)
                {
                    string nombreProducto = datos[0];
                    int CodigoProducto = int.Parse(datos[1]);
                    int CantidadVenta = int.Parse(datos[2]);
                    double PrecioUnitario = double.Parse(datos[3]);

                    _inventario.Add(new Productos(nombreProducto, CodigoProducto, CantidadVenta, PrecioUnitario));
                }
            }
        }
    }

    public void GuardarInventario(Productos productos)
    {
        _inventario.Add(productos);
        GuardarInventarioCompleto();
    }

    private void GuardarInventarioCompleto()
    {
        using (StreamWriter sw = new StreamWriter(_rutaArchivo, false))
        {
            foreach (var producto in _inventario)
            {
                sw.WriteLine($"{producto.nombreProducto},{producto.CodigoProducto},{producto.CantidadVenta},{producto.PrecioUnitario}");
            }
        }
    }

    public void mostrarInfo()
    {
        if (_inventario.Count > 0)
        {
            Console.WriteLine("\nInventario de productos:");
            Console.WriteLine("---------------------------------------------------");
            foreach (var producto in _inventario)
            {
                Console.WriteLine($"Nombre del Producto: {producto.nombreProducto}, Código del producto: {producto.CodigoProducto}, Cantidad en Ventas: {producto.CantidadVenta}, Precio unitario: {producto.PrecioUnitario:C}");
            }
            Console.WriteLine("---------------------------------------------------");
        }
        else
        {
            Console.WriteLine("No hay productos en el inventario");
        }
    }

    public Productos BuscarProductoPorCodigo(int codigo)
    {
        return _inventario.FirstOrDefault(p => p.CodigoProducto == codigo);
    }

    public bool ActualizarStock(int codigo, int cantidad)
    {
        var producto = BuscarProductoPorCodigo(codigo);
        if (producto != null && producto.CantidadVenta >= cantidad)
        {
            producto.CantidadVenta -= cantidad;
            GuardarInventarioCompleto();
            return true;
        }
        return false;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Bienvenido al sistema de inventario y ventas de productos");
        Registros miRegistro = new Registros("inventario.txt");

        while (true)
        {
            Console.WriteLine("\n------ MENÚ PRINCIPAL ------");
            Console.WriteLine("1. Añadir producto");
            Console.WriteLine("2. Comprar producto");
            Console.WriteLine("3. Ver inventario");
            Console.WriteLine("4. Mostrar Producto mas caro");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine() ?? string.Empty;

            switch (opcion)
            {
                case "1":
                    AñadirProducto(miRegistro);
                    break;
                case "2":
                    ComprarProducto(miRegistro);
                    break;
                case "3":
                    miRegistro.mostrarInfo();
                    break;
                case "4":
                    MostrarProductoMasCaro(miRegistro);
                    break;
                case "5":
                    Console.WriteLine("Gracias por usar el sistema. ¡Hasta pronto!");
                    return;
                default:
                    Console.WriteLine("Opción inválida. Por favor, intente de nuevo.");
                    break;
            }
        }
    }

    static void AñadirProducto(Registros registro)
    {
        Console.WriteLine("\n------ AÑADIR PRODUCTO ------");
        
        Console.Write("Nombre del producto: ");
        string nombreProducto = Console.ReadLine() ?? string.Empty;

        Console.Write("Código del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int codigoProducto))
        {
            Console.WriteLine("Código inválido. Se debe ingresar un número.");
            return;
        }

        // Verificar si el código ya existe
        if (registro.BuscarProductoPorCodigo(codigoProducto) != null)
        {
            Console.WriteLine("Error: Este código de producto ya existe.");
            return;
        }

        Console.Write("Cantidad de stock: ");
        if (!int.TryParse(Console.ReadLine(), out int CantidadVenta) || CantidadVenta < 0)
        {
            Console.WriteLine("Cantidad inválida. Se debe ingresar un número positivo.");
            return;
        }

        Console.Write("Precio unitario: ");
        if (!double.TryParse(Console.ReadLine(), out double precioUnitario) || precioUnitario <= 0)
        {
            Console.WriteLine("Precio inválido. Se debe ingresar un número positivo.");
            return;
        }

        Productos productos = new Productos(nombreProducto, codigoProducto, CantidadVenta, precioUnitario);
        registro.GuardarInventario(productos);
        Console.WriteLine("Producto añadido correctamente.");
    }

    static void MostrarProductoMasCaro(Registros registro)
    {
        Console.WriteLine("\n------ PRODUCTO MÁS CARO ------");

        if (registro.ObtenerInventario().Count == 0)
        {
            Console.WriteLine("No hay productos en el inventario.");
            return;
        }

        var productoMasCaro = registro.ObtenerInventario().OrderByDescending(p => p.PrecioUnitario).FirstOrDefault();

        if (productoMasCaro != null)
        {
            Console.WriteLine($"El producto más caro es: {productoMasCaro.nombreProducto}");
            Console.WriteLine($"Código: {productoMasCaro.CodigoProducto}");
            Console.WriteLine($"Precio: {productoMasCaro.PrecioUnitario:C}");
            Console.WriteLine($"Cantidad en stock: {productoMasCaro.CantidadVenta}");
        }
    }

    static void ComprarProducto(Registros registro)
    {
        Console.WriteLine("\n------ COMPRAR PRODUCTO ------");
        
        Console.Write("Ingrese el código del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int codigo))
        {
            Console.WriteLine("Código inválido. Se debe ingresar un número.");
            return;
        }

        var producto = registro.BuscarProductoPorCodigo(codigo);
        
        if (producto == null)
        {
            Console.WriteLine("Producto no encontrado. El código ingresado no existe.");
            return;
        }

        Console.WriteLine($"Producto: {producto.nombreProducto}");
        Console.WriteLine($"Precio: {producto.PrecioUnitario:C}");
        Console.WriteLine($"Stock disponible: {producto.CantidadVenta}");

        if (producto.CantidadVenta <= 0)
        {
            Console.WriteLine("Lo sentimos, no hay stock disponible para este producto.");
            return;
        }

        Console.Write("Cantidad a comprar: ");
        if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
        {
            Console.WriteLine("Cantidad inválida. Se debe ingresar un número positivo.");
            return;
        }

        if (cantidad > producto.CantidadVenta)
        {
            Console.WriteLine($"Lo sentimos, solo hay {producto.CantidadVenta} unidades disponibles.");
            return;
        }

        double totalCompra = producto.PrecioUnitario * cantidad;
        Console.WriteLine($"Total a pagar: {totalCompra:C}");

        Console.Write("Ingrese con cuánto va a pagar: ");
        if (!double.TryParse(Console.ReadLine(), out double pago) || pago < 0)
        {
            Console.WriteLine("Monto inválido. Se debe ingresar un número positivo.");
            return;
        }

        if (pago < totalCompra)
        {
            double faltante = totalCompra - pago;
            Console.WriteLine($"Monto insuficiente. Falta {faltante:C} para completar la compra.");
            return;
        }
        
        string jsonString = JsonSerializer.Serialize(registro.ObtenerInventario(), new JsonSerializerOptions { WriteIndented = true });
        if (registro.ActualizarStock(codigo, cantidad))
        {
            double cambio = pago - totalCompra;
            Console.WriteLine($"Compra realizada con éxito.");
            Console.WriteLine($"Su cambio es: {cambio:C}");
        }
        string jsonInventario = JsonSerializer.Serialize(registro.Inventario, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("Inventario.json", jsonInventario);
            Console.WriteLine("\nLos datos se han guardado correctamente en Inventario.json");

            Console.WriteLine("\nLECTURA DE ARCHIVO Y RESULTADOS:");
            Console.WriteLine("==============================");
            
            string jsonFromFile = File.ReadAllText("calificaciones.json");
            List<Productos> estudiantesLeidos = JsonSerializer.Deserialize<List<Productos>>(jsonFromFile);

            Console.WriteLine("\nombreProducto\t\tCodigoProducto\tCantidadVenta\tPrecioUnitario\tPROM\tESTADO");
            Console.WriteLine("--------------------------------------------------------");

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
    }
}