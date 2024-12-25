// using System;
// using System.Windows.Media.Imaging;
// using SharpDX;
// using SharpDX.Direct3D11;
// using SharpDX.D3DCompiler;
// using SharpDX.WIC;
// using SharpDX.Mathematics.Interop;
// using System.IO;
// using System.Windows.Media;
// using System.Runtime.InteropServices;
// using SharpDX.DXGI;
// using BitmapDecoder = SharpDX.WIC.BitmapDecoder;
// using BitmapSource = SharpDX.WIC.BitmapSource;
// using Device = SharpDX.Direct3D11.Device;
// using MapFlags = SharpDX.Direct3D11.MapFlags;
//
// public class ImageProcessor
// {
//     private Device device;
//     private DeviceContext context;
//
//     public ImageProcessor(Device device, DeviceContext context)
//     {
//         this.device = device;
//         this.context = context;
//     }
//
//     public BitmapSource ConvertToGrayscaleOnGPU(string imagePath)
//     {
//         // Step 1: Load image into texture
//         var texture = LoadTexture(imagePath);
//
//         // Step 2: Prepare compute shader for desaturation
//         var shader = PrepareComputeShader();
//
//         // Step 3: Dispatch the compute shader to process the image
//         DispatchComputeShader(texture, shader);
//
//         // Step 4: Read back the desaturated data from the GPU
//         byte[] data = ReadBackData(texture);
//
//         // Step 5: Convert the desaturated data to BitmapSource using WIC
//         return CreateBitmapSourceFromWicBitmap(data, texture.Description.Width, texture.Description.Height);
//     }
//
//     private Texture2D LoadTexture(string imagePath)
//     {
//         // Create the WIC factory
//         var factory = new ImagingFactory();
//
//         // Create a decoder from the file path
//         var decoder = new BitmapDecoder(factory, imagePath, DecodeOptions.CacheOnLoad);
//
//         // Get the first frame of the image
//         var bitmapFrame = decoder.GetFrame(0);
//
//         // Get image dimensions
//         int width = bitmapFrame.Size.Width;
//         int height = bitmapFrame.Size.Height;
//
//         // Create texture description for Direct3D texture
//         var textureDesc = new Texture2DDescription
//         {
//             Width = width,
//             Height = height,
//             MipLevels = 1,
//             ArraySize = 1,
//             Format = Format.B8G8R8A8_UNorm,
//             Usage = ResourceUsage.Default,
//             BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
//             CpuAccessFlags = CpuAccessFlags.None,
//             OptionFlags = ResourceOptionFlags.None,
//         };
//
//         // Create a new texture using the device
//         Texture2D texture = new Texture2D(device, textureDesc);
//
//         // You can optionally create a shader resource view to bind the texture
//         var textureView = new ShaderResourceView(device, texture);
//
//         // Return the texture
//         return texture;
//     }
//
//
//
//
//     private ShaderBytecode PrepareComputeShader()
//     {
//         // Simple HLSL shader for desaturating the image
//         string shaderCode = @"
//             RWTexture2D<float4> output : register(u0);
//             Texture2D<float4> input : register(t0);
//             SamplerState sam : register(s0);
//
//             [numthreads(16, 16, 1)]
//             void main(uint3 DTid : SV_DispatchThreadID)
//             {
//                 float4 color = input.Load(DTid.xy);
//                 float gray = dot(color.rgb, float3(0.2126, 0.7152, 0.0722));
//                 output[DTid.xy] = float4(gray, gray, gray, color.a);
//             }";
//
//         var shaderBytecode = ShaderBytecode.Compile(shaderCode, "main", "cs_5_0");
//         return shaderBytecode;
//     }
//
//     private void DispatchComputeShader(Texture2D texture, ShaderBytecode shaderBytecode)
//     {
//         // Set up the compute shader and dispatch
//         var computeShader = new ComputeShader(device, shaderBytecode);
//         context.ComputeShader.Set(computeShader);
//
//         var outputUAV = new UnorderedAccessView(device, texture);
//
//         // Bind the input texture and output UAV
//         context.SetShaderResources(0, texture);
//         context.SetUnorderedAccessViews(0, outputUAV);
//
//         // Dispatch compute shader (assuming texture size fits within 16x16 thread groups)
//         context.Dispatch(texture.Description.Width / 16, texture.Description.Height / 16, 1);
//     }
//
//     private byte[] ReadBackData(Texture2D texture)
//     {
//         // Create a staging buffer to read the data back from GPU
//         var stagingBufferDescription = new BufferDescription(
//             texture.Description.Width * texture.Description.Height * 4, 
//             ResourceUsage.Staging, 
//             BindFlags.None, 
//             CpuAccessFlags.Read, 
//             OptionFlags.None
//         );
//
//         var stagingBuffer = new Buffer(device, stagingBufferDescription);
//         context.CopyResource(texture, stagingBuffer);
//
//         DataStream dataStream;
//         context.MapSubresource(stagingBuffer, 0, MapMode.Read, MapFlags.None, out dataStream);
//
//         byte[] data = new byte[stagingBuffer.Description.SizeInBytes];
//         dataStream.Read(data, 0, data.Length);
//         context.UnmapSubresource(stagingBuffer, 0);
//
//         return data;
//     }
//
//     private BitmapSource CreateBitmapSourceFromWicBitmap(byte[] data, int width, int height)
//     {
//         // Convert the desaturated data to BitmapSource using WIC
//         var factory = new ImagingFactory();
//         var bitmap = new SharpDX.WIC.Bitmap(factory, width, height, PixelFormat.Format32bppRGBA, data);
//         return ConvertWicBitmapToBitmapSource(bitmap);
//     }
//
//     private BitmapSource ConvertWicBitmapToBitmapSource(SharpDX.WIC.Bitmap wicBitmap)
//     {
//         var frame = wicBitmap.GetFrame(0);
//         var stream = new MemoryStream();
//         var encoder = new PngBitmapEncoder();
//         encoder.Frames.Add(BitmapFrame.Create(frame));
//         encoder.Save(stream);
//         stream.Seek(0, SeekOrigin.Begin);
//
//         var bitmapSource = new BitmapImage();
//         bitmapSource.BeginInit();
//         bitmapSource.StreamSource = stream;
//         bitmapSource.EndInit();
//         
//         return bitmapSource;
//     }
// }
