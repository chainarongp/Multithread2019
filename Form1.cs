using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mutithread2019
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MySharedClass mySharedClass = new MySharedClass();
            mySharedClass.SetValue("100");
            mySharedClass.GetValue();
            
        }
    }

    interface IReadFromShared
    {
        string GetValue();
    }
    interface IWriteToShared
    {
        void SetValue(string value);
    }
    class MySharedClass : IReadFromShared, IWriteToShared
    {
        string _foo;
        public string GetValue()
        {
            return _foo;
        }
        public void SetValue(string value)
        {
            _foo = value;
        }
        void Foo(Synchronizer<MySharedClass, IReadFromShared, IWriteToShared> sync)
        {
            sync.Write(x =>
            {
                x.SetValue("new value");
            });
            sync.Reader(x =>
            {
                Console.WriteLine(x.GetValue());
            });
        }
        public void ExcuteParallel()
        {
           // var sync = new Synchronizer<string, string, string>("initail_");
          //  var actions = new Actions();

        }
    }

    public class Synchronizer<TImpl, TIRead, TIWrite> where TImpl : TIWrite, TIRead
    {
        ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        TImpl _shared;
        public Synchronizer(TImpl shared) { }
        public void Reader(Action<TIRead> functor)
        {
            _lock.EnterReadLock();
            try
            {
                functor(_shared);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        public void Write(Action<TIWrite> fucntor)
        {
            _lock.EnterWriteLock();
            try
            {
                fucntor(_shared);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

    }
}
