using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace SplitPics
{
	class SplitPic
	{
		[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
		private static extern void MoveMemory(IntPtr pbDestination, IntPtr pbSource, [MarshalAs(UnmanagedType.U4)] uint Length);

		[DllImport("Kernel32.dll", EntryPoint = "RtlFillMemory")]
		private static extern void FillMemory(IntPtr pbDestination, [MarshalAs(UnmanagedType.U4)] uint cb, [MarshalAs(UnmanagedType.U1)] byte bValue);

		public static void Split(string stFile)
		{
			Bitmap bmpIn = new Bitmap(stFile);
			PixelFormat fmt = bmpIn.PixelFormat;
			if (fmt != PixelFormat.Format24bppRgb)
				throw new ApplicationException("Unsupported bitmap format.");
			int dyOut = (bmpIn.Height + 1) / 2;
			Bitmap bmpOut0 = new Bitmap(bmpIn.Width, dyOut, fmt);
			Bitmap bmpOut1 = new Bitmap(bmpIn.Width, dyOut, fmt);
			Rectangle rect = new Rectangle(0, 0, bmpIn.Width, bmpIn.Height);
			BitmapData dataIn = bmpIn.LockBits(rect, ImageLockMode.ReadOnly, fmt);
			rect = new Rectangle(0, 0, bmpIn.Width, dyOut);
			BitmapData dataOut0 = bmpOut0.LockBits(rect, ImageLockMode.WriteOnly, fmt);
			BitmapData dataOut1 = bmpOut1.LockBits(rect, ImageLockMode.WriteOnly, fmt);
			IntPtr pbIn = dataIn.Scan0;
			IntPtr pbOut0 = dataOut0.Scan0;
			IntPtr pbOut1 = dataOut1.Scan0;
			uint cb1Pix = 3; // only 24-bit RGB is supported
			uint cbRow = cb1Pix * (uint)dataIn.Width;
			if (dataOut0.Stride != dataIn.Stride || dataOut1.Stride != dataIn.Stride)
				throw new ApplicationException("Unexpected bitmap layout mismatch");
			for (int y = 0; y < dataIn.Height; y += 2)
			{
				MoveMemory(pbOut0, pbIn, cbRow);
				pbIn = new IntPtr(pbIn.ToInt64() + dataIn.Stride);
				pbOut0 = new IntPtr(pbOut0.ToInt64() + dataIn.Stride);
				if (y < dataIn.Height)
				{
					MoveMemory(pbOut1, pbIn, cbRow);
					pbIn = new IntPtr(pbIn.ToInt64() + dataIn.Stride);
					pbOut1 = new IntPtr(pbOut1.ToInt64() + dataIn.Stride);
				}
				else
				{
					FillMemory(pbOut1, cbRow, 0);
				}
			}
			bmpIn.UnlockBits(dataIn);
			bmpOut0.UnlockBits(dataOut0);
			bmpOut1.UnlockBits(dataOut1);
			string stFileBase = Path.ChangeExtension(stFile, null);
			bmpOut0.Save(stFileBase + ".0.bmp", ImageFormat.Bmp);
			bmpOut1.Save(stFileBase + ".1.bmp", ImageFormat.Bmp);
			bmpIn.Dispose();
			bmpOut0.Dispose();
			bmpOut1.Dispose();
		}

		public static void SplitFiles(string stDirectory)
		{
			string[] rgstFiles = Directory.GetFiles(stDirectory, "*.bmp");
			int cFiles = rgstFiles.Length;
			for (int i = 0; i < cFiles; i++)
			{
				string stFile = Path.Combine(stDirectory, rgstFiles[i]);
				Split(stFile);
			}
		}
	}
}
