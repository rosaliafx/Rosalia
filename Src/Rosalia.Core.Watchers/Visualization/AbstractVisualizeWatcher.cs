namespace Rosalia.Core.Watchers.Visualization
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Rosalia.Core.Watchers.Visualization.Layout;
    using Rosalia.Core.Watching;

    public abstract class AbstractVisualizeWatcher : IWorkflowWatcher
    {
        public void Register(IWorkflowEventsAware eventsAware)
        {
            var taskIndex = 1;
            var rootComposite = (Composite) null;

            eventsAware.WorkflowStart += (sender, args) =>
            {
                rootComposite = TaskToComposite(args.RootTask);
            };

            eventsAware.TaskCompleteExecution += (sender, args) =>
            {
                var targetComposite = rootComposite.FindItemByTaskId(args.CurrentTask.Id);
                targetComposite.Result = args.Result;
                targetComposite.Order = taskIndex;

                taskIndex++;
            };

            eventsAware.WorkflowComplete += (sender, args) =>
            {
                
                LayoutData layoutData = null;

                using (var font = new Font(FontFamily.GenericMonospace, 12))
                using (var tempBitmap = new Bitmap(1, 1))
                using (var tempGraphics = Graphics.FromImage(tempBitmap))
                {
                    var layoutManager = new LayoutManager(new GrapthRenderer(tempGraphics, font))
                    {
                        HorisontalMargin = 20,
                        VerticalMargin = 30,
                        LastLayerLeftMargin = 0
                    };

                    layoutData = layoutManager.CalcLayout(rootComposite);
                }

                IEnumerable<IDisposable> resources = AllocateResources(layoutData);
                RenderStart(layoutData);

                foreach (var layoutItem in layoutData.AllItems)
                {
                    RenderItem((Composite) layoutItem.Data, layoutItem);
                }

                foreach (var link in layoutData.AllLinks)
                {
                    RenderLink(link);
                }

                RenderEnd();

                foreach (var disposable in resources)
                {
                    disposable.Dispose();
                }
            };
        }

        protected abstract void RenderEnd();

        protected abstract void RenderStart(LayoutData layoutData);

        protected abstract void RenderLink(Link link);

        protected abstract void RenderItem(Composite data, LayoutItem layoutItem);

        protected abstract IDisposable[] AllocateResources(LayoutData layoutData);

        private Composite TaskToComposite(IIdentifiable task)
        {
            var composite = new Composite(task);
            foreach (var child in task.IdentifiableChildren)
            {
                composite.AddChild(TaskToComposite(child));
            }

            return composite;
        }
    }
}