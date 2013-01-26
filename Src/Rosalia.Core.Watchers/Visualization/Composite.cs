namespace Rosalia.Core.Watchers.Visualization
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Watchers.Visualization.Layout;

    public class Composite : IComposite
    {
        private readonly IList<IComposite> _children = new List<IComposite>();
        //private readonly Composite _parent;
        private readonly IIdentifiable _task;

        public Composite(IIdentifiable task/*, Composite parent*/)
        {
            _task = task;
            //_parent = parent;
        }

        public IEnumerable<IComposite> ChildrenItems
        {
            get { return _children; }
        }

//        public Composite Parent
//        {
//            get { return _parent; }
//        }


        public ExecutionResult Result { get; set; }

        public int Order { get; set; }

        public IIdentifiable Task
        {
            get { return _task; }
        }

        public void AddChild(IComposite child)
        {
            _children.Add(child);
        }

        public Composite FindItemByTaskId(Guid taskId)
        {
            if (_task.Id == taskId)
            {
                return this;
            }

            foreach (Composite child in _children)
            {
                var childResult = child.FindItemByTaskId(taskId);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            return null;
        }
    }
}