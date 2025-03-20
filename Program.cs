
//OLIVER GARCIA AGUADO
//ALICIA SARAHI SANCHEZ VARELA

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace hidato
{
    class Program
    {

        struct Posicion
        {
            public int fil, col;
        };
        struct Casilla
        {
            public int num;   // número de la casilla (-1: no jugable, 0: vacía)
            public bool fija; // Indica si la casilla es fija (true) o editable (false)
        };
        struct Tablero
        {
            public Casilla[,] cas; // matriz de casillas
            public Posicion cursor;     // posición actual del cursor
        };


        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            bool enPartida = true;

            int[,] mat = {
                { 0, 33, 35,  0,  0, -1, -1, -1},
                { 0,  0, 24, 22,  0, -1, -1, -1},
                { 0,  0,  0, 21,  0,  0, -1, -1},
                { 0, 26,  0, 13, 40, 11, -1, -1},
                {27,  0,  0,  0,  9,  0,  1, -1},
                {-1, -1,  0,  0, 18,  0,  0, -1},
                {-1, -1, -1, -1,  0,  7,  0,  0},
                {-1, -1, -1, -1, -1, -1,  5,  0}
            };
            Tablero tablero;
            Console.WriteLine(" ▄  █ ▄█ ██▄   ██     ▄▄▄▄▀ ████▄ \r\n█   █ ██ █  █  █ █ ▀▀▀ █    █   █ \r\n██▀▀█ ██ █   █ █▄▄█    █    █   █ \r\n█   █ ▐█ █  █  █  █   █     ▀████ \r\n   █   ▐ ███▀     █  ▀            \r\n  ▀              █                \r\n                ▀                ");
            Console.WriteLine("Pulse 0 si quiere empezar con una matriz base o 1 si quiere cargar desde archivo");
            string selection = Console.ReadLine();
            if (selection == "1")
            {
                Console.WriteLine("Escriba el nombre del archivo que desea cargar");
                string archivo = Console.ReadLine();
                Inicializa(CargaMatriz(archivo), out tablero);
            }
            else if (selection == "0") Inicializa(mat, out tablero);
            else
            {
                Console.WriteLine("Selección no valida, se carga la matriz por defecto");
                Console.ReadLine();
                Inicializa(mat, out tablero);
            }
            int ultimaCas = Ultimo(tablero);
            char tecla = ' ';
            Console.Clear();
            Render(tablero, tecla);
            Posicion choque; // almacena cual es la posicion de ruptura del bucle Comprueba
            bool continuar = true; //Mientras el juego no esté completado o el jugador no aborte, continuará estará en true

            

            while (continuar)
            {
                tecla = LeeInput();
                while (tecla == ' ')
                {
                    tecla = LeeInput();
                }
                if (tecla != 'q')
                {
                    ActualizaEstado(tecla, ref tablero);
                    continuar = !Comprueba(tablero, out choque, ultimaCas);
                    Console.Clear();
                    Render(tablero, tecla);
                    if (tecla == 'c') RenderCont(tablero, choque);
                    Console.SetCursorPosition(0, 20);
                }
                else continuar = false;
                

            }
            if (Comprueba(tablero, out choque, ultimaCas))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" _  _  __   __    __  __  __  _  __  __   __     _ _  \r\n| || |/  \\/' _/  / _]/  \\|  \\| |/  \\| _\\ /__\\   / / \\ \r\n| >< | /\\ `._`. | [/| /\\ | | ' | /\\ | v | \\/ |  \\_\\_/ \r\n|_||_|_||_|___/  \\__|_||_|_|\\__|_||_|__/ \\__/   (_(_) ");
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" __  _  _ ___ __  __     __  __  __  ___ _____ __  __   __    \r\n|_ \\| || | __/ _]/__\\   /  \\|  \\/__\\| _ |_   _/  \\| _\\ /__\\   \r\n _\\ | \\/ | _| [/| \\/ | | /\\ | -| \\/ | v / | || /\\ | v | \\/ |  \r\n/___|\\__/|___\\__/\\__/  |_||_|__/\\__/|_|_\\ |_||_||_|__/ \\__/   ");
            }

            
            
        }
        static int[,] CargaMatriz(string file)
        {
            StreamReader flujo = new StreamReader(file);
            string[] dimensiones = flujo.ReadLine().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            int[,] MatrizCargada = new int[int.Parse(dimensiones[0]), int.Parse(dimensiones[1])];
            for (int i = 0; i < MatrizCargada.GetLength(0); i++)
            {
                string[] nums = flujo.ReadLine().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < MatrizCargada.GetLength(1); j++)
                {
                    MatrizCargada[i, j] = int.Parse(nums[j]);
                }
            }
            flujo.Close();
            return MatrizCargada;
        }
        static void ActualizaEstado(char c, ref Tablero tab)
        {
            int dimension = tab.cas.GetLength(0);
            bool IsCharNumber = false;
            Casilla currentCasilla = tab.cas[tab.cursor.fil, tab.cursor.col];

            if (c == 'u' && tab.cursor.fil > 0 && tab.cas[tab.cursor.col, tab.cursor.fil - 1].num != -1)
            {
                tab.cursor.fil--;
            }
            else if (c == 'd' && tab.cursor.fil < dimension - 1 && tab.cas[tab.cursor.col, tab.cursor.fil + 1].num != -1)
            {
                tab.cursor.fil++;
            }
            else if (c == 'l' && tab.cursor.col > 0 && tab.cas[tab.cursor.col - 1, tab.cursor.fil].num != -1)
            {
                tab.cursor.col--;
            }
            else if (c == 'r' && tab.cursor.col < dimension - 1 && tab.cas[tab.cursor.col + 1, tab.cursor.fil].num != -1)
            {
                tab.cursor.col++;
            }

            else if (!currentCasilla.fija)
            {
                if (char.IsDigit(c))
                {
                    int casteado = Convert.ToInt32(new string(c, 1));
                    CambiarCasilla(ref tab.cas[tab.cursor.fil, tab.cursor.col], casteado);
                }
            }

        }
        static void CambiarCasilla(ref Casilla casilla, int numero)
        {
            if (casilla.num == 0 || casilla.num > 9)
            {
                casilla.num = numero;
            }
            else casilla.num = casilla.num * 10 + numero;
        }
        static Posicion Busca1(Tablero tab)
        {
            int i = 0;
            int j = 0;
            Posicion ubicacion;
            bool found = false;
            ubicacion.fil = -1;
            ubicacion.col = -1; //aunque vaya a encontrar la ubicación de 1 se necesita asignar el struct para poder retornarlo
            while (!found && i < tab.cas.GetLength(1))
            {
                while (!found && j < tab.cas.GetLength(1))
                {
                    if (tab.cas[i, j].num == 1 && tab.cas[i, j].fija)
                    {
                        ubicacion.fil = i;
                        ubicacion.col = j;
                        found = true;
                    }
                    j++;
                }
                j = 0;
                i++;
            }
            return ubicacion;

        }
        static int Ultimo(Tablero tab)
        {
            int ultimo = 0;
            for (int i = 0; i < tab.cas.GetLength(0); i++)
            {
                for (int j = 0; j < tab.cas.GetLength(1); j++)
                {
                    if (tab.cas[i, j].num > ultimo) ultimo = tab.cas[i, j].num;
                }
            }
            return ultimo;
        }
        static bool Siguiente(Tablero tab, Posicion pos, out Posicion pos1)
        {
            bool encontrado = false;
            pos1.fil = -1;
            pos1.col = -1;

            int i = -1;
            while (i <= 1 && !encontrado)
            {
                int j = -1;
                while (j <= 1 && !encontrado)
                {
                    if (!(i == 0 && j == 0))
                    {
                        int newFil = pos.fil + i;
                        int newCol = pos.col + j;

                        if (newFil >= 0 && newFil < tab.cas.GetLength(0) && newCol >= 0 && newCol < tab.cas.GetLength(1))
                        {
                            if (tab.cas[newFil, newCol].num == tab.cas[pos.fil, pos.col].num + 1)
                            {
                                pos1.fil = newFil;
                                pos1.col = newCol;
                                encontrado = true;
                            }
                        }
                    }
                    j++;
                }
                i++;
            }

            return encontrado;
        }
        static bool Comprueba(Tablero tab, out Posicion pos, int ultimaCas)
        {
            bool solucionado = false;
            pos.col = pos.fil = -1;
            Posicion analized = Busca1(tab);

            int lastFound = tab.cas[analized.fil, analized.col].num;
            bool encuentraSiguiente = true;

            while (lastFound < ultimaCas && encuentraSiguiente)
            {

                Posicion nextpos;

                if (Siguiente(tab, analized, out nextpos))
                {
                    if (tab.cas[nextpos.fil, nextpos.col].num == lastFound + 1)
                    {
                        analized = nextpos;
                        lastFound = tab.cas[nextpos.fil, nextpos.col].num;
                        encuentraSiguiente = true;
                    }
                }

                else
                {
                    pos.col = analized.col;
                    pos.fil = analized.fil;
                    encuentraSiguiente = false;
                }
            }
            if (encuentraSiguiente) solucionado = true;
            else solucionado = false;
            return solucionado;
        }
        static char LeeInput()
        {
            char d = ' ';

            if (Console.KeyAvailable)
            {
                string tecla = Console.ReadKey(true).Key.ToString();
                switch (tecla)
                {

                    /* INPUTS ELEMENTALES PARA EL JUEGO BÁSICO */

                    // movimiento del cursor 	
                    case "LeftArrow": d = 'l'; break;
                    case "UpArrow": d = 'u'; break;
                    case "RightArrow": d = 'r'; break;
                    case "DownArrow": d = 'd'; break;

                    // comprobar tablero 
                    case "c": case "C": d = 'c'; break;

                    // terminar juego
                    case "Escape": case "q": case "Q": d = 'q'; break;

                    // ver posibles valores en posicion	
                    default:
                        if (tecla.Length == 2 && tecla[0] == 'D' && tecla[1] >= '0' && tecla[1] <= '9')
                            d = tecla[1];
                        break;
                }
            }
            return d;
        }
        static void Inicializa(int[,] mat, out Tablero tab)
        {
            tab.cursor.fil = tab.cursor.col = 0;
            tab.cas = new Casilla[mat.GetLength(0), mat.GetLength(1)];
            for (int i = 0; i < tab.cas.GetLength(0); i++)
            {
                for (int j = 0; j < tab.cas.GetLength(1); j++)
                {
                    tab.cas[i, j].num = mat[i, j];
                    if (tab.cas[i, j].num != 0) tab.cas[i, j].fija = true;
                    else tab.cas[i, j].fija = false;

                }
                Console.WriteLine("");
            }

        }
        static void Render(Tablero tablero, char tecla)
        {
            Casilla selectedCas = tablero.cas[tablero.cursor.fil, tablero.cursor.col];
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < tablero.cas.GetLength(0); i++)
            {
                for (int j = 0; j < tablero.cas.GetLength(1); j++)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    DibujaCasilla(tablero.cas[i, j]);
                }
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.SetCursorPosition(tablero.cursor.col * 4, tablero.cursor.fil * 2);
            if (selectedCas.fija)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.White;
                DibujaCasilla(tablero.cas[tablero.cursor.fil, tablero.cursor.col]);

            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Green;
                DibujaCasilla(tablero.cas[tablero.cursor.fil, tablero.cursor.col]);
            }
        }
        static void DibujaCasilla(Casilla casilla)
        {
            if (casilla.num == -1)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("  ");
            }
            else if (casilla.num == 0)
            {
                Console.Write("  ");
            }
            else
            {
                if (casilla.fija) Console.ForegroundColor = ConsoleColor.Black;
                else Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{casilla.num,2}");
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("  ");
        }
        static void RenderCont(Tablero tab, Posicion pos)
        {
            Console.SetCursorPosition(pos.col * 4, pos.fil * 2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            DibujaCasilla(tab.cas[pos.fil, pos.col]);
            Console.BackgroundColor = ConsoleColor.Black;

        }
    }
}
