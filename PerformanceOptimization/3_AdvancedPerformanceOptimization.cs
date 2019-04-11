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


		static long MeasureA(int size)
		{
			byte[] image = new byte[size * size * 3];

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < image.Length;)
			{
				byte grey = (byte)(.299 * image[i + 2] + .587 * image[i + 1] + .114 * image[i]);
				image[i] = grey;
				image[i + 1] = grey;
				image[i + 2] = grey;
				i += 3;
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static unsafe long MeasureB(int size)
		{
			byte[] image = new byte[size * size * 3];

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			unsafe
			{
				// fixed means the GC cannot move it while control is in the scope
				fixed (byte* p = &image[0])
				{
					for (int i = 0; i < image.Length;)
					{
						byte grey = (byte)(.299 * image[i + 2] + .587 * image[i + 1] + .114 * image[i]);
						p[i] = grey;
						p[i + 1] = grey;
						p[i + 2] = grey;
						i += 3;
					}
				}
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static unsafe long MeasureC(int size)
		{
			byte[] image = new byte[size * size * 3];

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			unsafe
			{
				fixed (byte* imgPtr = &image[0])
				{
					byte* p = imgPtr;
					int stopAddress = (int)p + size * size * 3;
					while ((int)p != stopAddress)
					{
						byte grey = (byte)(.299 * p[2] + .587 * p[1] + .114 * p[0]);
						*p = grey;
						// this is faster because CPU has specialized instruction for incrementing by one
						*(p + 1) = grey;
						*(p + 2) = grey;
						p += 3;
					}
				}
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		public static void ImageProcessingV2()
		{
			// we measure the performance for simulated images of different sizes
			for (int size = 512; size < 4096; size += 128)
			{
				// image processing using byte[]
				long duration1 = MeasureA(size);

				// image processing using byte* and reading by indexer[]
				long duration2 = MeasureB(size);

				// image processing using byte* and advancing the pointer
				long duration3 = MeasureC(size);

				// write results
				Console.WriteLine($"{size}\t{duration1}\t{duration2}\t{duration3}");

				/* Results
					MeasureC is around 25% faster
					Basically we should avoid pointers only in specific scenarios it makes sense to use them
				 */
			}
		}
	}
}