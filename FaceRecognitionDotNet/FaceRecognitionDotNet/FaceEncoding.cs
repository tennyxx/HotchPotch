﻿using System;
using System.Runtime.Serialization;
using DlibDotNet;

namespace FaceRecognitionDotNet
{

    /// <summary>
    /// Represents a feature data of face. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class FaceEncoding : IDisposable, ISerializable
    {

        #region Fields

        [NonSerialized]
        private readonly Matrix<double> _Encoding;

        #endregion

        #region Constructors

        internal FaceEncoding(Matrix<double> encoding)
        {
            this._Encoding = encoding;
        }

        private FaceEncoding(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            var array = (double[])info.GetValue(nameof(this._Encoding), typeof(double[]));
            var row = (int)info.GetValue(nameof(this._Encoding.Rows), typeof(int));
            var column = (int)info.GetValue(nameof(this._Encoding.Columns), typeof(int));
            this._Encoding = new Matrix<double>(array, row, column);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this object has been disposed of.
        /// </summary>
        public bool IsDisposed
        {
            get;
            private set;
        }

        internal Matrix<double> Encoding => this._Encoding;

        /// <summary>
        /// Gets the size of feature data.
        /// </summary>
        public int Size
        {
            get
            {
                if(this.IsDisposed)
                    throw new ObjectDisposedException($"{nameof(FaceEncoding)}");
                return this._Encoding.Size;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by this <see cref="FaceEncoding"/>.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        /// <summary>
        /// Releases all resources used by this <see cref="FaceEncoding"/>.
        /// </summary>
        /// <param name="disposing">Indicate value whether <see cref="IDisposable.Dispose"/> method was called.</param>
        private void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.IsDisposed = true;

            if (disposing)
            {
                this._Encoding?.Dispose();
            }

        }

        #endregion

        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this._Encoding), this._Encoding.ToArray());
            info.AddValue(nameof(this._Encoding.Rows), this._Encoding.Rows);
            info.AddValue(nameof(this._Encoding.Columns), this._Encoding.Columns);
        }

        #endregion

    }

    public class FaceEncodingData {
        public FaceEncodingData() { }
        public FaceEncodingData(FaceEncoding faceEncoding, double distance) {
            this.faceEncoding = faceEncoding;
            this.distance = distance;
        }
        public FaceEncoding faceEncoding { get; set; }
        public double distance { get;set;}
    }

}
