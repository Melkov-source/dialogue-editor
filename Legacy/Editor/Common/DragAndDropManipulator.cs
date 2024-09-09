using System.Collections.Generic;
using System.Linq;
using History.Editor.HistoryEditor.Common;
using History.Editor.HistoryEditor.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class DragAndDropManipulator : PointerManipulator
    {
        private readonly ExplorerChildBase _child;

        private List<ExplorerFolder> _currentFolders;
        private List<ExplorerFile> _currentFiles;

        public DragAndDropManipulator(ExplorerChildBase child)
        {
            _child = child;

            var root = child.ChildInfo.Root;

            target = root;

            _root = root.parent;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        private Vector2 _targetStartPosition;
        private Vector3 _pointerStartPosition;

        private bool _isEnabled;

        private readonly VisualElement _root;

        private void PointerDownHandler(PointerDownEvent evt)
        {
            _targetStartPosition = target.transform.position;
            _pointerStartPosition = evt.position;
            target.CapturePointer(evt.pointerId);
            
            _currentFolders =  Explorer
                .GetEntities(ExplorerEntityType.Folder)
                .Select(child => (ExplorerFolder)child)
                .ToList();

            _currentFiles = Explorer
                .GetEntities(ExplorerEntityType.File)
                .Select(child => (ExplorerFile)child)
                .ToList();

            _currentFiles.Remove((ExplorerFile)_child);

            _isEnabled = true;
        }

        private ExplorerFolder _currentSelectedDropFolder;
        private ExplorerFile _currentSelectedSwapFile;

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (_isEnabled == false || !target.HasPointerCapture(evt.pointerId))
            {
                return;
            }

            var pointerDelta = evt.position - _pointerStartPosition;

            var targetPosition = new Vector2(_targetStartPosition.x + pointerDelta.x,
                _targetStartPosition.y + pointerDelta.y);

            target.transform.position = targetPosition;

            MoveToFolderHeader();
            MoveToFileHeader();
        }

        private void MoveToFileHeader()
        {
            var overlapsFolder = _currentFolders.FindAll(folder => OverlapsTarget(folder.Content));
            var overlappedFolder = FindClosest(overlapsFolder);

            if (overlappedFolder == default)
            {
                foreach (var file in _currentFiles)
                {
                    file.HeaderUpHover.UnsetLine();
                    file.HeaderDownHover.UnsetLine();
                }
                return;
            }

            if (_child.ChildInfo.Parent.AssetId.Equals(overlappedFolder.AssetId) == false)
            {
                foreach (var file in _currentFiles)
                {
                    file.HeaderUpHover.UnsetLine();
                    file.HeaderDownHover.UnsetLine();
                }
                return;
            }
            
            var overlaps = _currentFiles.FindAll(file => OverlapsTarget(file.Header));
            var overlappedFile = FindClosest(overlaps);

            if (overlappedFile != default)
            {
                var headerHovers = new List<HeaderHover>
                {
                    overlappedFile.HeaderUpHover,
                    overlappedFile.HeaderDownHover
                };

                var overlappedHover = headerHovers.FirstOrDefault(hover => OverlapsTarget(hover.Root));

                if (overlappedHover == default)
                {
                    foreach (var file in _currentFiles)
                    {
                        file.HeaderUpHover.UnsetLine();
                        file.HeaderDownHover.UnsetLine();
                    }
                    return;
                }

                foreach (var hover in headerHovers)
                {
                    if (hover == overlappedHover)
                    {
                        foreach (var file in _currentFiles)
                        {
                            file.HeaderUpHover.UnsetLine();
                            file.HeaderDownHover.UnsetLine();
                        }
                        
                        hover.SetLine();
                    }
                    else
                    {
                        hover.UnsetLine();
                    }
                }
            }
            else
            {
                foreach (var file in _currentFiles)
                {
                    file.HeaderUpHover.UnsetLine();
                    file.HeaderDownHover.UnsetLine();
                }
            
                _currentSelectedSwapFile = default;
            }
        }

        private void MoveToFolderHeader()
        {
            var overlaps = _currentFolders.FindAll(folder => OverlapsTarget(folder.Header));
            var overlappedFolder = FindClosest(overlaps);

            if (overlappedFolder != default)
            {
                if (_currentSelectedDropFolder == default)
                {
                    _currentSelectedDropFolder = overlappedFolder;
                }
                else
                {
                    if (_currentSelectedDropFolder.AssetId.Equals(overlappedFolder.AssetId))
                    {
                        return;
                    }
                }
                
                _currentSelectedDropFolder = overlappedFolder;
                
                foreach (var folder in _currentFolders)
                {
                    folder.UnsetDropLine();
                }
                
                overlappedFolder.SetDropLine();
            }
            else
            {
                foreach (var folder in _currentFolders)
                {
                    folder.UnsetDropLine();
                }
            
                _currentSelectedDropFolder = default;
            }
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (_isEnabled && target.HasPointerCapture(evt.pointerId))
            {
                target.ReleasePointer(evt.pointerId);
            }
        }

        private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
        {
            if (_isEnabled)
            {
                if ((Vector2)target.transform.position == _targetStartPosition)
                {
                    _isEnabled = false;
                    return;
                }

                var overlaps = _currentFolders.FindAll(folder => OverlapsTarget(folder.Header));
                var overlappedFolder = FindClosest(overlaps);

                if (overlappedFolder != default)
                {
                    overlappedFolder.Add(_child);
                }

                target.transform.position = Vector3.zero;

                _isEnabled = false;
            }

            foreach (var folder in _currentFolders)
            {
                folder.UnsetDropLine();
            }
            
            foreach (var file in _currentFiles)
            {
                file.HeaderUpHover.UnsetLine();
                file.HeaderDownHover.UnsetLine();
            }
            
            _currentSelectedDropFolder = default;
        }

        private bool OverlapsTarget(VisualElement element)
        {
            return target.worldBound.Overlaps(element.worldBound);
        }

        private TChild FindClosest<TChild>(IEnumerable<TChild> folders) where TChild : ExplorerChildBase
        {
            var bestDistanceSq = float.MaxValue;

            TChild closest = null;

            foreach (var folder in folders)
            {
                var displacement = GetLocalWorldPosition(folder.Header) - target.transform.position;
                var distanceSq = displacement.sqrMagnitude;

                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    closest = folder;
                }
            }

            return closest;
        }

        private VisualElement FindClosest(List<VisualElement> elements)
        {
            var bestDistanceSq = float.MaxValue;

            VisualElement closest = null;

            foreach (var element in elements)
            {
                var displacement = GetLocalWorldPosition(element) - target.transform.position;
                var distanceSq = displacement.sqrMagnitude;

                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    closest = element;
                }
            }

            return closest;
        }
        
        private TChild FindClosest<TChild>(List<TChild> elements) where TChild : IHandlerVisualElement
        {
            var bestDistanceSq = float.MaxValue;

            IHandlerVisualElement closest = null;

            foreach (var element in elements)
            {
                var displacement = GetLocalWorldPosition(element.Root) - target.transform.position;
                var distanceSq = displacement.sqrMagnitude;

                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    closest = element;
                }
            }

            return (TChild)closest;
        }

        private Vector3 GetLocalWorldPosition(VisualElement element)
        {
            var worldPosition = element.parent.LocalToWorld(element.layout.position);

            return _root.WorldToLocal(worldPosition);
        }
    }
}