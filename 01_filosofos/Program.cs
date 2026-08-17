using System;
using System.Threading;



Cena main = new();

await main.start();


public class Fork{

    public bool onUse = false;

}


public class Filosofo{
    public int filling = 0;
    public bool filled = false;

    public async Task eat(int n){

        if (filled) return;

        Console.WriteLine($"Filosofo {n} comiendo");
        
        await Task.Delay(2000);

        Console.WriteLine($"Filosofo {n} ha terminado");
        filling += 1;

        if( filling >= 3){
            Console.WriteLine($"Filosofo {n} lleno");   
            filled = true;
        }

    }

}


public class Cena{

    private int n = 5;
    private Fork[] forks = new Fork[5];
    private Filosofo[] filosofos =new Filosofo[5];
    private static readonly SemaphoreSlim sem = new SemaphoreSlim(initialCount: 3, maxCount: 3);
    private int count = 0;

    public Cena(){
        
        for (int i = 0; i < 5; i++){
            forks[i] = new Fork();
            filosofos[i] = new Filosofo();
        }
            

    }

    public async Task start()
    {
        while (count < 5)
        {
            Task[] tareas =
            {
                comer(0),
                comer(1),
                comer(2),
                comer(3),
                comer(4)
            };

            await Task.WhenAll(tareas);

            count = 0;

            for (int i = 0; i < 5; i++)
            {
                if (filosofos[i].filled)
                    count++;
            }
        }
    }

     public async Task comer(int id){

        int left = id;
        int right = (id+1)%5;

        await sem.WaitAsync();

        try
        {
            // Intentar tomar el Fork izquierdo
            if (forks[left].onUse)
                return;

            forks[left].onUse = true;

            // Intentar tomar el Fork derecho
            if (forks[right].onUse)
            {
                forks[left].onUse = false;
                return;
            }

            forks[right].onUse = true;

            // Ya tiene ambos forks
            await filosofos[id].eat(id);

            // Soltar ambos forks
            forks[left].onUse = false;
            forks[right].onUse = false;
        }
        finally
        {
            sem.Release();
        }



    }





}


