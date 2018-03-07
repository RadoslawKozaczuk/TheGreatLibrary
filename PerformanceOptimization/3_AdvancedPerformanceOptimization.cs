using System;
using System.Diagnostics;

// not supported in .Net Core
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Runtime.Remoting.Messaging;

namespace PerformanceOptimization
{
	class AdvancedPerformanceOptimization
	{
		const int Repetitions = 1000;

		static long ArrayMeasure(int elements)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			int[] list = new int[elements];
			for (int r = 0; r < Repetitions; r++)
				for (int i = 0; i < elements; i++)
					list[i] = i;

			stopwatch.Stop();
			return stopwatch.ElapsedTicks;
		}

		static long StackallocMeasure(int elements)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			unsafe
			{
				int* list = stackalloc int[elements];
				for (int r = 0; r < Repetitions; r++)
					for (int i = 0; i < elements; i++)
						list[i] = i;
			}

			stopwatch.Stop();
			return stopwatch.ElapsedTicks;
		}

		public static void ArrayOnTheStack()
		{
			// Note: stack size equals to 1 MB for 32-bit processes and 4 MB for 64-bit processes
			Console.WriteLine("ele\tstalloc\tint[]");

			long sumArray = 0;
			long sumStackalloc = 0;

			// measurement run
			for (int elements = 10_000; elements <= 100_000; elements += 10_000)
			{
				long duration1 = ArrayMeasure(elements);
				long duration2 = StackallocMeasure(elements);
				Console.WriteLine($"{elements}\t{duration1}\t{duration2}");

				sumArray += duration1;
				sumStackalloc += duration2;
			}

			Console.WriteLine($"Stackallock performance as a % of the int[] { sumStackalloc * 100 / sumArray }");

			/* Results:
				- stackalloc is barely better. Taking into consideration its drawbacks it should be not used at all.
				- stackalloc is present in the language as a low level interface (gateway) to other languages
			 */
		}

		// not supported in .Net Core
		//private static long MeasureA(Bitmap bmp)
		//{
		//	Stopwatch stopwatch = new Stopwatch();
		//	stopwatch.Start();
		//	for (int x = 0; x < bmp.Width; x++)
		//	{
		//		for (int y = 0; y < bmp.Height; y++)
		//		{
		//			Color pixel = bmp.GetPixel(x, y);
		//			byte grey = (byte)(.299 * pixel.R + .587 * pixel.G + .114 * pixel.B);
		//			Color greyPixel = Color.FromArgb(grey, grey, grey);
		//			bmp.SetPixel(x, y, greyPixel);
		//		}
		//	}
		//	stopwatch.Stop();
		//	return stopwatch.ElapsedMilliseconds;
		//}

		// not supported in .Net Core
		//private static long MeasureB(Bitmap bmp)
		//{
		//	BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
		//		ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
		//	Stopwatch stopwatch = new Stopwatch();

		//	unsafe
		//	{
		//		stopwatch.Start();
		//		byte* p = (byte*)(void*)bmData.Scan0.ToPointer();
		//		int stopAddress = (int)p + bmData.Stride * bmData.Height;
		//		while ((int)p != stopAddress)
		//		{
		//			byte pixel = (byte)(.299 * p[2] + .587 * p[1] + .114 * p[0]);
		//			*p = pixel;
		//			p++;
		//			*p = pixel;
		//			p++;
		//			*p = pixel;
		//			p++;
		//		}
		//		stopwatch.Stop();
		//	}
		//	bmp.UnlockBits(bmData);
		//	return stopwatch.ElapsedMilliseconds;
		//}

		// not supported in .Net Core
		public static void ImageProcessing()
		{
			Console.WriteLine("This example does not provide any output, please check the code.");

			//// grayscale conversion using getpixel & setpixel
			//Bitmap bmpA = (Bitmap)Bitmap.FromFile("lenna.png");
			//long elapsedA = MeasureA(bmpA);
			//bmpA.Save("lenna_bw_a.png");

			//// grayscale conversion using pointers
			//Bitmap bmpB = (Bitmap)Bitmap.FromFile("lenna.png");
			//long elapsedB = MeasureB(bmpB);
			//bmpB.Save("lenna_bw_b.png");

			//// write results
			//Console.WriteLine("Grayscale conversion using GetPixel/SetPixel: " + elapsedA);
			//Console.WriteLine("Grayscale conversion using pointers: " + elapsedB);
		}
	}
}