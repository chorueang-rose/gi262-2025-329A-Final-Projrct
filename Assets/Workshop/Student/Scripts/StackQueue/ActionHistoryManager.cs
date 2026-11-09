using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solution
{
    public class ActionHistoryManager : MonoBehaviour
    {
        // 1. Undo System using Stack
        private Stack<Vector2> undoStack = new Stack<Vector2>();
        private Stack<Vector2> redoStack = new Stack<Vector2>();

        // 2. Auto-Move System using Queue
        private Queue<Vector2> autoMoveQueue = new Queue<Vector2>();

        #region Undo / Redo Functions

        /// <summary>
        /// Saves the current player state (position) to the undo stack.
        /// </summary>
        public void SaveStateForUndo(Vector2 currentPosition)
        {
            if (undoStack.Count == 0 || !undoStack.Peek().Equals(currentPosition))
            {
                undoStack.Push(currentPosition);

                if (redoStack.Count > 0)
                {
                    redoStack.Clear();
                    Debug.Log("New move detected. Redo stack cleared.");
                }

                Debug.Log($"State saved: Position {currentPosition}. Stack size: {undoStack.Count}");
            }
        }

        /// <summary>
        /// Reverts the player's state to the previous one using the undo stack.
        /// </summary>
        public void UndoLastMove(OOPPlayer player)
        {
            if (player == null) return;

            if (undoStack.Count > 1)
            {
                // Pop Current Position
                Vector2 currentState = undoStack.Pop();
                redoStack.Push(currentState);

                // Target Previous Position
                Vector2 previousState = undoStack.Peek();

                int toX = (int)previousState.x;
                int toY = (int)previousState.y;

                player.UpdatePosition(toX, toY);
                Debug.Log($"Undo successful! Reverted to position: {previousState}. Stack size: {undoStack.Count}");
            }
            else
            {
                Debug.Log("Cannot undo: No previous states saved.");
            }
        }

        /// <summary>
        /// Redoes the last undone move using the redo stack.
        /// </summary>
        public void RedoLastMove(OOPPlayer player)
        {
            if (player == null) return;

            if (redoStack.Count > 0)
            {
                Vector2 stateToRedo = redoStack.Pop();
                undoStack.Push(stateToRedo);

                int toX = (int)stateToRedo.x;
                int toY = (int)stateToRedo.y;

                player.UpdatePosition(toX, toY);
                Debug.Log($"Redo successful! Reverted to position: {stateToRedo}. Undo size: {undoStack.Count}, Redo size: {redoStack.Count}");
            }
            else
            {
                Debug.Log("Cannot redo: No undone states available.");
            }
        }

        #endregion

        #region Auto-Move Functions

        public void StartAutoMoveSequence(OOPPlayer player)
        {
            if (player == null) return;

            List<Vector2> sequence = new List<Vector2>
            {
                Vector2.right,
                Vector2.right,
                Vector2.up,
                Vector2.left,
                Vector2.up,
                Vector2.down
            };

            StartCoroutine(ProcessAutoMoveSequence(sequence, player));
        }

        public IEnumerator ProcessAutoMoveSequence(List<Vector2> moves, OOPPlayer player)
        {
            if (player == null) yield break;

            player.isAutoMoving = true;
            autoMoveQueue.Clear();

            foreach (var move in moves)
            {
                autoMoveQueue.Enqueue(move);
            }

            Debug.Log($"Auto-move sequence started with {autoMoveQueue.Count} steps.");

            while (autoMoveQueue.Count > 0)
            {
                if (player == null) break;

                Vector2 nextDirection = autoMoveQueue.Dequeue();
                player.Move((int)nextDirection.x, (int)nextDirection.y);

                yield return new WaitForSeconds(0.5f);
            }

            if (player != null)
            {
                player.isAutoMoving = false;
            }

            Debug.Log("Auto-move sequence finished.");
        }

        internal void RecordMove(int positionX, int positionY)
        {
            SaveStateForUndo(new Vector2(positionX, positionY));
        }

        #endregion
    }
}
