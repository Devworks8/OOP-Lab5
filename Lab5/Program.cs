using System;

using Psim.Particles;
using Psim.ModelComponents;
using Psim.Materials;

namespace Psim
{
	class Program
	{
		static void Main(string[] args)
		{
			DispersionData dData;
			dData.LaData = new double[] { -2.22e-7, 9260.0, 0.0};
			dData.TaData = new double[] { -2.28e-7, 5240.0, 0.0};
			dData.WMaxLa = 7.63916048e13;
			dData.WMaxTa = 3.0100793072e13;

			RelaxationData rData;
			rData.Bl = 1.3e-24;
			rData.Btn = 9e-13;
			rData.Btu = 1.9e-18;
			rData.BI = 1.2e-45;
			rData.W = 2.42e13;

			Material silicon = new Material(in dData, in rData);

			Sensor s1 = new Sensor(1, silicon, 300);
			Cell c1 = new Cell(10, 10, s1);
			Sensor s2 = new Sensor(2, silicon, 300);
			Cell c2 = new Cell(1, 1, s2);

			Phonon p1 = new Phonon(1);
			p1.SetCoords(2, 2);
			p1.SetDirection(0, 1);
			p1.DriftTime = 1;
			p1.Update(1, 10, Polarization.LA);

            // Test transition surface (HandlePhonon)
            Console.WriteLine("Transitory tests\n");
			
			c1.AddPhonon(p1);

            //1 phonon in cell 1
            Console.WriteLine(c1);

			c1.SetTransitionSurface(SurfaceLocation.top, c2);
			c1.GetSurface((SurfaceLocation)c1.MoveToNearestSurface(p1)).HandlePhonon(p1);
			
			//1 incoming phonon in cell 2
			Console.WriteLine(c2);

			c2.MergeIncPhonons();
			c1.Phonons.Remove(p1);
			
			//1 phonon in cell 2
            Console.WriteLine(c2);

			//Move and transition a phonon from cell 1 to cell 2 using the bottom transitory surface
			p1.SetDirection(0, -1);
			p1.DriftTime = 1;
			p1.Update(1, 10, Polarization.LA);

			c2.SetTransitionSurface(SurfaceLocation.bot, c1);
			c2.GetSurface((SurfaceLocation)c2.MoveToNearestSurface(p1)).HandlePhonon(p1);

			//1 incoming phonon in cell 1
			Console.WriteLine(c1);

			c1.MergeIncPhonons();

			//1 phonon in cell 1
			Console.WriteLine(c1);

			//Move a phonon to a boundary surface
			p1.SetDirection(1, 0);
			p1.DriftTime = 1;
			p1.Update(1, 10, Polarization.LA);

			c1.GetSurface((SurfaceLocation)c1.MoveToNearestSurface(p1)).HandlePhonon(p1);

			//1 phonon in cell 1
			Console.WriteLine(c1);

            // Test emit surface (HandlePhonon)

            Console.WriteLine("\nEmission test\n");
			//Move a phonon to an emitting surface
			p1.SetCoords(2, 2);
			p1.SetDirection(-1, 0);
			p1.DriftTime = 1;
			p1.Update(1, 10, Polarization.LA);

            Console.WriteLine("Phonon active: {0}", p1.Active);

			c1.SetEmitSurface(SurfaceLocation.left, c1, 300);
			c1.SetEmitPhonons(290, 300, 0.01);

			if (!(c1.MoveToNearestSurface(p1) is null))
			{
				c1.GetSurface((SurfaceLocation)c1.MoveToNearestSurface(p1)).HandlePhonon(p1);
			}

			Console.WriteLine("Phonon active: {0}", p1.Active);

			Console.ReadKey(true);
		}
	}
}
