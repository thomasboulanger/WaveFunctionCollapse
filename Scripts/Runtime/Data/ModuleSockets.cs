using NaughtyAttributes;
using UnityEngine;

[SelectionBase]
public class ModuleSockets : MonoBehaviour
{
    public const float MODULE_SIZE = 3;

    [Min(.1f)]
    public float weight = 1;

    [NaughtyAttributes.EnumFlags]
    public ModuleFlag flag = (ModuleFlag)(-1);

    [Socket(SocketAttribute.ESocketMode.Side)]
    public string xPrev, xNext;

    [Socket(SocketAttribute.ESocketMode.Vertical)]
    public string yPrev, yNext;

    [Socket(SocketAttribute.ESocketMode.Side)]
    public string zPrev, zNext;

#if UNITY_EDITOR
    private static bool _drawSockets = true;

    [Button]
    private void ToggleSocketVisibility()
    {
        _drawSockets = !_drawSockets;
    }
    
    private void OnDrawGizmos ()
    {
        void ShowLabel (string socket, Vector3 offset, GUIStyle style)
        {
            SocketUtility.ParseSocketIndex(socket, out int id);
            Random.InitState(id);

            Color color = (SocketUtility.IsInvalid(socket) || SocketUtility.IsWildCard(socket)) ?
                    Color.red
                    : Random.ColorHSV(0,1, .5f,1, 1,1);
            style.normal.textColor = color;

            style.active = style.normal;
            style.hover = style.normal;
            style.focused = style.normal;

            UnityEditor.Handles.color = color;

            if (SocketUtility.IsInvalid(socket) || SocketUtility.IsWildCard(socket))
                DrawInvalid(offset);

            else if (SocketUtility.IsSymmetric(socket))
                DrawSymmetry(color, offset);

            else if (SocketUtility.IsDirectional(socket) && SocketUtility.ParseSocketLastDigit(socket, out int angle))
                DrawQuarter(color, offset, angle);

            else if (SocketUtility.IsFlippable(socket))
                DrawFlip(color, offset, SocketUtility.IsFlipped(socket));


            UnityEditor.Handles.Label(transform.position + MODULE_SIZE * .55f * offset, SocketUtility.IsWildCard(socket) ? "?" : socket, style);
        }

        void DrawInvalid (Vector3 offset)
        {
            Gizmos.color = Color.red;

            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position + MODULE_SIZE / 2f * offset, Quaternion.Euler(-90,0,0), new Vector3(900, 900, 1.0f));
            Gizmos.DrawFrustum(Vector3.zero, 90, .001f, 0, 1.0f);

            Gizmos.matrix = oldMatrix;
        }

        void DrawQuarter (Color color, Vector3 offset, int angle)
        {
            angle *= 90;
            Vector3 normal = offset.normalized;
            Vector3 right = Vector3.right;
            float dot = Vector3.Dot(Vector3.up, normal);
            if (Mathf.Abs(dot) < .75f)
                right = Vector3.up;
            else
                normal = Vector3.up;

            Vector3 from = Quaternion.AngleAxis(angle, normal) * right;
            Vector3 to = Quaternion.AngleAxis(90, normal) * from;

            Gizmos.color = color;

            Gizmos.DrawLine(transform.position + MODULE_SIZE / 2f * offset, transform.position + MODULE_SIZE / 2f * offset + from / 2);
            Gizmos.DrawLine(transform.position + MODULE_SIZE / 2f * offset, transform.position + MODULE_SIZE / 2f * offset + to / 2);
            UnityEditor.Handles.DrawWireArc(transform.position + MODULE_SIZE / 2f * offset, normal, from, 90, MODULE_SIZE / 4f);
        }

        void DrawFlip (Color color, Vector3 offset, bool flipped)
        {
            Vector3 normal = offset.normalized;
            float dot = Vector3.Dot(Vector3.up, normal);

            // If SIDE socket
            if (Mathf.Abs(dot) < .75f) {
                Vector3 from = Vector3.up;
                Vector3 to = -from;

                Gizmos.color = color;

                Gizmos.DrawLine(transform.position + MODULE_SIZE / 2f * offset + from / 2, transform.position + MODULE_SIZE / 2f * offset + to / 2);
                UnityEditor.Handles.DrawWireArc(transform.position + MODULE_SIZE / 2f * offset, normal, flipped ? from : to, 180, MODULE_SIZE / 4f);
            }

            // If VERTICAL socket
            else {
                UnityEditor.Handles.DrawWireCube(
                    transform.position + MODULE_SIZE / 2f * offset,
                    new Vector3(flipped ? 1f : .5f, 0, !flipped ? 1f : .5f) * MODULE_SIZE * .8f);

            }
        }

        void DrawSymmetry (Color color, Vector3 offset)
        {
            Vector3 normal = offset.normalized;

            UnityEditor.Handles.DrawWireDisc(
                transform.position + MODULE_SIZE / 2f * offset,
                offset.normalized,
                MODULE_SIZE / 4f);
        }


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(MODULE_SIZE, MODULE_SIZE, MODULE_SIZE));

        if (!_drawSockets) return;

        GUIStyle style = new GUIStyle("label") { alignment = TextAnchor.MiddleCenter, fontSize = 20 };

        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

        ShowLabel(xPrev, -Vector3.right, style);
        ShowLabel(xNext, Vector3.right, style);
        ShowLabel(yPrev, -Vector3.up, style);
        ShowLabel(yNext, Vector3.up, style);
        ShowLabel(zPrev, -Vector3.forward, style);
        ShowLabel(zNext, Vector3.forward, style);

    }
#endif
}
