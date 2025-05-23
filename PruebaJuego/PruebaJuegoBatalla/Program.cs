﻿using System;

namespace JuegoNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bienvenido al juego!");

            Personaje sacerdote = new Sacerdote("Samson", 30, 5);
            Personaje barbaro = new Barbaro("Dave", 30, 7, 10);

            Equipo tunica = new Armadura(5);
            Equipo hacha = new Arma(6);

            sacerdote.Equipar(tunica);
            barbaro.Equipar(hacha);

            Personaje? ganador = Juego.Batalla(barbaro, sacerdote);

            if (ganador != null)
            {
                Console.WriteLine($"\n{ganador.GetNombre()} se recupera para la siguiente batalla...");
                ganador.RegenerarVida();

                Console.WriteLine("\n¡El ganador se enfrenta al nuevo retador!");
                Console.WriteLine("Un nuevo retador aparece entre las tinieblas...");

                Personaje nuevoRival = new PersonajeAdicional();
                Equipo lanza = new Arma(4);
                nuevoRival.Equipar(lanza);

                Juego.Batalla(ganador, nuevoRival);
            }
            else
            {
                Console.WriteLine("\n No hay ganador, no se puede continuar con la batalla.");
            }
        }
    }

    class Personaje
    {
        private string nombre;
        private int vida;
        private int vidaMaxima;
        private int ataque;
        private Equipo equipo;
        private static Random random = new Random();

        public Personaje(string nombre, int vida, int ataque)
        {
            this.nombre = nombre;
            this.vida = vida;
            this.vidaMaxima = vida;
            this.ataque = ataque;
        }

        public string Nombre
        {
            get => nombre;
            set => nombre = value;
        }

        public int Vida
        {
            get => vida;
            set
            {
                vida = value;
                vidaMaxima = value;
            }
        }

        public int Ataque
        {
            get => ataque;
            set => ataque = value;
        }

        public string GetNombre() => nombre;
        public int GetVida() => vida;
        public int GetAtaque() => ataque;

        public virtual void Atacar(Personaje objetivo)
        {
            int danioBase = ataque + (equipo?.GetModificadorAtaque() ?? 0);

            if (random.Next(100) < 10)
            {
                Console.WriteLine($"{nombre} falló el ataque.");
                Console.ReadKey();
                return;
            }

            int danio = random.Next(danioBase / 2, danioBase + 1);

            if (random.Next(100) < 10)
            {
                danio *= 2;
                Console.WriteLine("Golpe crítico. " + nombre + " hace daño doble.");
                Console.ReadKey();
            }

            objetivo.RecibirDanio(danio);
        }

        public virtual void RecibirDanio(int danio)
        {
            vida -= danio;
            if (vida < 0) vida = 0;
        }

        public void Equipar(Equipo equipo)
        {
            this.equipo = equipo;
        }

        public void RegenerarVida()
        {
            this.vida = this.vidaMaxima;
            Console.WriteLine($"{nombre} Se come un pan con queso y una coquita para restaurar su vida a {vidaMaxima} puntos.");
            Console.ReadKey();
        }
    }

    class Barbaro : Personaje
    {
        private int furia;
        private static Random random = new Random();

        public Barbaro(string nombre, int vida, int ataque, int furia)
            : base(nombre, vida, ataque)
        {
            this.furia = furia;
        }

        public override void Atacar(Personaje objetivo)
        {
            int danioBase = GetAtaque() + (furia / 2);

            if (random.Next(100) < 10)
            {
                Console.WriteLine($"{GetNombre()} falló el ataque.");
                Console.ReadKey();
                return;
            }

            int danio = random.Next(danioBase / 2, danioBase + 1);

            if (random.Next(100) < 10)
            {
                danio *= 2;
                Console.WriteLine("Golpe crítico. Dave hace daño doble.");
                Console.ReadKey();
            }

            objetivo.RecibirDanio(danio);
        }
    }

    class Sacerdote : Personaje
    {
        private static Random random = new Random();

        public Sacerdote(string nombre, int vida, int ataque)
            : base(nombre, vida, ataque) { }

        public override void RecibirDanio(int danio)
        {
            int defensaExtra = 2;
            base.RecibirDanio(Math.Max(danio - defensaExtra, 0));
        }

        public void AtacarEspecial(Personaje objetivo)
        {
            base.Atacar(objetivo);
            if (random.Next(100) <= 30)
            {
                Console.WriteLine("El sacerdote lanza un hechizo doble.");
                Console.ReadKey();
                base.Atacar(objetivo);
            }
        }

        public override void Atacar(Personaje objetivo)
        {
            AtacarEspecial(objetivo);
        }
    }

    class PersonajeAdicional : Personaje
    {
        
        public PersonajeAdicional() : base("", 0, 0)
        {
            string nombre = ObtenerNombreValido();
            int vida = ObtenerEnteroValido("vida", 10, 35);
            int ataque = ObtenerEnteroValido("ataque", 1, 9);

            this.Nombre = nombre;
            this.Vida = vida;
            this.Ataque = ataque;
        }

        private string ObtenerNombreValido()
        {
            while (true)
            {
                Console.Write("Ingrese el nombre del nuevo guerrero (obligatorio): ");
                string? entrada = Console.ReadLine();
                Console.WriteLine($"Su nombre es  {entrada} y viene a partir craneos.");
                Console.ReadKey();

                if (!string.IsNullOrWhiteSpace(entrada) && entrada.All(char.IsLetter))
                {
                    return entrada;
                }

                Console.WriteLine("El pj requiere un nombre para poder luchar, ingrese solo letras.");
            }
        }
        

        private int ObtenerEnteroValido(string campo, int minimo, int maximo)
        {
            while (true)
            {
                Console.Write($"Ingrese {campo} (entre {minimo} y {maximo}): ");
                string? entrada = Console.ReadLine();

                if (int.TryParse(entrada, out int valor) && valor >= minimo && valor <= maximo)
                {
                    return valor;
                }

                Console.WriteLine($"Número invalido, {campo} debe ser entre {minimo} y {maximo}. Si no estaria demasiado cheto");
            }
        }
    }

    class Equipo
    {
        private int modificadorAtaque;
        private int modificadorArmadura;

        public Equipo(int modificadorAtaque, int modificadorArmadura)
        {
            this.modificadorAtaque = modificadorAtaque;
            this.modificadorArmadura = modificadorArmadura;
        }

        public int GetModificadorAtaque() => modificadorAtaque;
        public int GetModificadorArmadura() => modificadorArmadura;
    }

    class Arma : Equipo
    {
        public Arma(int modificadorAtaque)
            : base(modificadorAtaque, 0) { }
    }

    class Armadura : Equipo
    {
        public Armadura(int modificadorArmadura)
            : base(0, modificadorArmadura) { }
    }

    class Juego
    {
        private static Random random = new Random();

        public static Personaje? Batalla(Personaje p1, Personaje p2)
        {
            Console.WriteLine($"\nComienza la gran batalla entre {p1.GetNombre()} y {p2.GetNombre()}");
            Console.ReadKey();

            bool turnoP1 = random.Next(2) == 0;

            while (p1.GetVida() > 0 && p2.GetVida() > 0)
            {
                int vidaAntesP1 = p1.GetVida();
                int vidaAntesP2 = p2.GetVida();

                if (turnoP1)
                {
                    EjecutarTurno(p1, p2);
                    if (p2.GetVida() > 0)
                        EjecutarTurno(p2, p1);
                }
                else
                {
                    EjecutarTurno(p2, p1);
                    if (p1.GetVida() > 0)
                        EjecutarTurno(p1, p2);
                }

                if (p1.GetVida() <= 0 && p2.GetVida() <= 0)
                {
                    Console.WriteLine("\nAmbos luchadores se han matado entre sí.");
                    Console.ReadKey();
                    return null;
                }
            }

            Personaje ganador = p1.GetVida() > 0 ? p1 : p2;
            Console.WriteLine($"\n{ganador.GetNombre()} ha ganado la batalla");
            Console.ReadKey();
            return ganador;
        }

        private static void EjecutarTurno(Personaje atacante, Personaje defensor)
        {
            Console.WriteLine($"{atacante.GetNombre()} ataca a {defensor.GetNombre()}.");
            Console.ReadKey();
            atacante.Atacar(defensor);
            Console.WriteLine($"{defensor.GetNombre()} tiene {defensor.GetVida()} de vida restante.");
            Console.ReadKey();

            if (defensor.GetVida() <= 0)
            {
                Console.WriteLine($"{defensor.GetNombre()} ha sido derrotado y exhala su último aliento.");
                Console.ReadKey();
            }
        }
    }
}
