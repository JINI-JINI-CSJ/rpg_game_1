using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSystem
{
    // ─────────────────────────────────────────────
    //  위치(Position) 객체
    // ─────────────────────────────────────────────
    [Serializable]
    public class QTPoint
    {
        public Vector2 Position { get; private set; }
        public object Data { get; set; }   // 원하는 페이로드를 붙일 수 있습니다

        // csj 추가
        public float BucketDist;

        public int tag_hash;

        public QuadTreeNode quadTreeNode;

        public QTPoint(Vector2 position, int _tag_hash , object data = null)
        {
            Position = position;
            Data     = data;
            tag_hash = _tag_hash;
        }

        public QTPoint(float x, float y, int _tag_hash ,  object data = null)
            : this(new Vector2(x, y), _tag_hash , data) { }

        // ── 유틸리티 ──────────────────────────────
        /// <summary>다른 QTPoint까지의 거리</summary>
        public float DistanceTo(QTPoint other)
            => Vector2.Distance(Position, other.Position);

        /// <summary>다른 QTPoint까지의 거리 제곱(비교용, sqrt 생략)</summary>
        public float SqrDistanceTo(QTPoint other)
            => (Position - other.Position).sqrMagnitude;

        /// <summary>원 안에 포함되는지 여부</summary>
        public bool IsInsideCircle(Vector2 center, float radius)
            => Vector2.Distance(Position, center) <= radius;

        /// <summary>AABB 사각형 안에 포함되는지 여부</summary>
        public bool IsInsideRect(Rect rect)
            => rect.Contains(Position);

        public override string ToString()
            => $"QTPoint({Position.x:F2}, {Position.y:F2})";

        // csj 추가
        // 버킷 들어갈때 호출
        virtual public void OnEnterBucketDist(){}

        public Vector3 Vector3()
        {
            return new Vector3( Position.x , 0 , Position.y );
        }
    }


    // ─────────────────────────────────────────────
    //  AABB 경계 박스
    // ─────────────────────────────────────────────
    [Serializable]
    public readonly struct AABB
    {
        public readonly Vector2 Center;
        public readonly Vector2 HalfSize;   // 가로/세로 각 절반 크기

        public float Left   => Center.x - HalfSize.x;
        public float Right  => Center.x + HalfSize.x;
        public float Bottom => Center.y - HalfSize.y;
        public float Top    => Center.y + HalfSize.y;

        public AABB(Vector2 center, Vector2 halfSize)
        {
            Center   = center;
            HalfSize = halfSize;
        }

        public AABB(float cx, float cy, float hw, float hh)
            : this(new Vector2(cx, cy), new Vector2(hw, hh)) { }

        // ── 유틸리티 ──────────────────────────────
        /// <summary>점이 경계 내부에 있는지 확인 (경계선 포함)</summary>
        public bool Contains(Vector2 point)
            =>  point.x >= Left  && point.x <= Right
             && point.y >= Bottom && point.y <= Top;

        public bool Contains(QTPoint point) => Contains(point.Position);

        /// <summary>다른 AABB와 겹치는지 확인</summary>
        public bool Intersects(AABB other)
            =>  Mathf.Abs(Center.x - other.Center.x) <= HalfSize.x + other.HalfSize.x
             && Mathf.Abs(Center.y - other.Center.y) <= HalfSize.y + other.HalfSize.y;

        /// <summary>원과 겹치는지 확인</summary>
        public bool IntersectsCircle(Vector2 circleCenter, float radius)
        {
            float dx = Mathf.Abs(circleCenter.x - Center.x);
            float dy = Mathf.Abs(circleCenter.y - Center.y);

            if (dx > HalfSize.x + radius || dy > HalfSize.y + radius) return false;
            if (dx <= HalfSize.x || dy <= HalfSize.y) return true;

            float cornerDistSq = Mathf.Pow(dx - HalfSize.x, 2)
                                + Mathf.Pow(dy - HalfSize.y, 2);
            return cornerDistSq <= radius * radius;
        }

        /// <summary>UnityEngine.Rect 로 변환</summary>
        public Rect ToRect()
            => new Rect(Left, Bottom, HalfSize.x * 2f, HalfSize.y * 2f);

        /// <summary>4분면 중심 좌표 반환 (NW, NE, SW, SE 순)</summary>
        public (Vector2 NW, Vector2 NE, Vector2 SW, Vector2 SE) QuadrantCenters()
        {
            float qx = HalfSize.x * 0.5f;
            float qy = HalfSize.y * 0.5f;
            return (
                new Vector2(Center.x - qx, Center.y + qy),  // NW
                new Vector2(Center.x + qx, Center.y + qy),  // NE
                new Vector2(Center.x - qx, Center.y - qy),  // SW
                new Vector2(Center.x + qx, Center.y - qy)   // SE
            );
        }

        public override string ToString()
            => $"AABB(center:{Center}, half:{HalfSize})";
    }


    // ─────────────────────────────────────────────
    //  쿼드트리 노드
    // ─────────────────────────────────────────────
    public class QuadTreeNode
    {
        // ── 설정 상수 ──────────────────────────────
        private const int DEFAULT_CAPACITY = 4;   // 분할 전 최대 포인트 수
        private const int MAX_DEPTH        = 8;   // 최대 분할 깊이

        // ── 필드 ───────────────────────────────────
        public AABB              Boundary  { get; private set; }
        public int               Depth     { get; private set; }
        public bool              Divided   { get; private set; }

        private readonly int          _capacity;
        private readonly List<QTPoint> _points;

        public object Data;

        // 자식 노드 (분할 후 생성)
        public QuadTreeNode NW { get; private set; }
        public QuadTreeNode NE { get; private set; }
        public QuadTreeNode SW { get; private set; }
        public QuadTreeNode SE { get; private set; }


        // ── 생성자 ─────────────────────────────────
        public QuadTreeNode(AABB boundary, int capacity = DEFAULT_CAPACITY, int depth = 0)
        {
            Boundary  = boundary;
            _capacity = capacity;
            Depth     = depth;
            _points   = new List<QTPoint>(capacity);
        }


        // ─────────────────────────────────────────
        //  핵심 메서드
        // ─────────────────────────────────────────

        /// <summary>포인트 삽입. 성공 시 true 반환.</summary>
        public bool Insert(QTPoint point)
        {
            if (!Boundary.Contains(point)) return false;

            if (!Divided)
            {
                if (_points.Count < _capacity || Depth >= MAX_DEPTH)
                {
                    _points.Add(point);
                    point.quadTreeNode = this;
                    return true;
                }
                Subdivide();
            }

            return NE.Insert(point) || NW.Insert(point)
                || SE.Insert(point) || SW.Insert(point);
        }

        /// <summary>AABB 범위 내 모든 포인트 조회</summary>
        public List<QTPoint> Query(AABB range, int _tag_hash = 0 , List<QTPoint> found = null)
        {
            found ??= new List<QTPoint>();

            if (!Boundary.Intersects(range)) return found;

            foreach (var p in _points)
            {
                if( _tag_hash != 0 )
                {
                    if ( _tag_hash == p.tag_hash && range.Contains(p)) found.Add(p);
                }
                else
                {
                    if (range.Contains(p)) found.Add(p);
                }
            }
                

            if (Divided)
            {
                NW.Query(range,_tag_hash, found);
                NE.Query(range,_tag_hash, found);
                SW.Query(range,_tag_hash, found);
                SE.Query(range,_tag_hash, found);
            }
            return found;
        }

        /// <summary>원형 범위 내 모든 포인트 조회</summary>
        public List<QTPoint> QueryCircle(Vector2 center, float radius , int _tag_hash = 0 , List<QTPoint> found = null)
        {
            found ??= new List<QTPoint>();

            if (!Boundary.IntersectsCircle(center, radius)) return found;

            float sqrRadius = radius * radius;
            foreach (var p in _points)
            {
                if( _tag_hash != 0 )
                {
                    if ( _tag_hash == p.tag_hash &&
                        (p.Position - center).sqrMagnitude <= sqrRadius
                        )
                        found.Add(p);                   
                }
                else
                {
                    if ((p.Position - center).sqrMagnitude <= sqrRadius)
                        found.Add(p);                    
                }
            }


            if (Divided)
            {
                NW.QueryCircle(center, radius,_tag_hash, found);
                NE.QueryCircle(center, radius,_tag_hash, found);
                SW.QueryCircle(center, radius,_tag_hash, found);
                SE.QueryCircle(center, radius,_tag_hash, found);
            }
            return found;
        }

        /// <summary>특정 포인트에서 가장 가까운 포인트 탐색 (Nearest Neighbor)</summary>
        public QTPoint FindNearest(Vector2 target, ref float bestDist , int tag_hash , QTPoint best = null)
        {
            // 경계와의 최소 거리가 현재 최선보다 멀면 스킵
            if (!CouldContainNearer(target, bestDist)) return best;

            foreach (var p in _points)
            {
                if( tag_hash != 0 )
                {
                    if( p.tag_hash != tag_hash ) continue;
                }

                float d = (p.Position - target).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best     = p;
                }
            }

            if (Divided)
            {
                // 가까운 분면 먼저 방문
                var children = SortChildrenByDistance(target);
                foreach (var child in children)
                    best = child.FindNearest(target, ref bestDist, tag_hash , best);
            }
            return best;
        }

        /// <summary>포인트 제거. 성공 시 true 반환.</summary>
        public bool Remove(QTPoint point)
        {
            if (!Boundary.Contains(point)) return false;

            if (_points.Remove(point)) return true;

            if (Divided)
                return NW.Remove(point) || NE.Remove(point)
                    || SW.Remove(point) || SE.Remove(point);

            return false;
        }

        /// <summary>트리 전체 초기화</summary>
        public void Clear()
        {
            _points.Clear();
            NW = NE = SW = SE = null;
            Divided = false;
        }

        /// <summary>트리 내 총 포인트 수</summary>
        public int Count()
        {
            int total = _points.Count;
            if (Divided)
                total += NW.Count() + NE.Count() + SW.Count() + SE.Count();
            return total;
        }

        /// <summary>트리 내 모든 포인트 수집</summary>
        public List<QTPoint> GetAllPoints( int _tag_hash , List<QTPoint> result = null)
        {
            result ??= new List<QTPoint>();
            if( _tag_hash != 0 )
            {
                foreach( var s in _points )
                    if( s.tag_hash == _tag_hash )result.Add(s);
            }
            else
            {
                result.AddRange(_points);                
            }

            if (Divided)
            {
                NW.GetAllPoints(_tag_hash , result);
                NE.GetAllPoints(_tag_hash , result);
                SW.GetAllPoints(_tag_hash , result);
                SE.GetAllPoints(_tag_hash , result);
            }
            return result;
        }

        /// <summary>포인트 위치 갱신 (이동 처리)</summary>
        public bool UpdatePoint(QTPoint point, Vector2 newPosition)
        {
            if (!Remove(point)) return false;
            point = new QTPoint(newPosition,point.tag_hash ,point.Data);
            return Insert(point);
        }

        /// <summary>K개의 최근접 이웃 탐색 (KNN)</summary>
        public List<QTPoint> FindKNearest(Vector2 target, int k , int _tag_hash)
        {
            var all    = GetAllPoints(_tag_hash);
            var result = new List<QTPoint>(all);
            result.Sort((a, b) =>
                (a.Position - target).sqrMagnitude
                    .CompareTo((b.Position - target).sqrMagnitude));

            return result.Count <= k ? result : result.GetRange(0, k);
        }

        /// <summary>현재 노드의 최대 깊이 반환</summary>
        public int MaxDepth()
        {
            if (!Divided) return Depth;
            return Mathf.Max(NW.MaxDepth(), NE.MaxDepth(),
                             SW.MaxDepth(), SE.MaxDepth());
        }

        // ─────────────────────────────────────────
        //  카메라 시야(Frustum) 내 최하위 노드 수집
        // ─────────────────────────────────────────

        /// <summary>
        /// 카메라 시야(Frustum)와 겹치는 최하위(Leaf) 쿼드트리 노드를 모두 수집합니다.
        ///
        /// ▸ 2D 직교(Orthographic) 카메라 전용입니다.
        ///   Perspective 카메라는 아래 <see cref="GetLeafNodesInFrustum(Plane[], List{QuadTreeNode})"/> 오버로드를 사용하세요.
        ///
        /// 동작 원리:
        ///   카메라의 orthographicSize, aspect, transform 으로 뷰 AABB 를 계산한 뒤
        ///   재귀적으로 트리를 내려가면서, 분할되지 않은(Leaf) 노드만 결과 목록에 추가합니다.
        ///   중간 노드는 뷰 AABB 와 교차할 때만 자식으로 내려가므로 불필요한 탐색을 최소화합니다.
        /// </summary>
        /// <param name="camera">시야를 기준으로 사용할 유니티 카메라</param>
        /// <param name="result">결과를 누적할 리스트 (null 이면 내부에서 생성)</param>
        /// <returns>시야 안에 포함된 Leaf 노드 목록</returns>
        public List<QuadTreeNode> GetLeafNodesInView(Camera camera, List<QuadTreeNode> result = null)
        {
            result ??= new List<QuadTreeNode>();

            // ── 카메라 시야를 2D AABB 로 변환 ────────────────────────────
            AABB viewAABB = CameraToAABB(camera);

            CollectLeafNodesInAABB(viewAABB, result);
            return result;
        }

        /// <summary>
        /// 카메라의 Frustum Plane 배열로 최하위(Leaf) 노드를 수집합니다.
        /// Perspective(원근) 카메라 또는 카메라 외의 임의 절두체에 사용하세요.
        ///
        /// 사용 예:
        /// <code>
        ///   Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        ///   var leaves = rootNode.GetLeafNodesInFrustum(planes);
        /// </code>
        /// </summary>
        /// <param name="frustumPlanes">GeometryUtility.CalculateFrustumPlanes() 결과</param>
        /// <param name="result">결과를 누적할 리스트 (null 이면 내부에서 생성)</param>
        /// <returns>Frustum 안에 포함된 Leaf 노드 목록</returns>
        public List<QuadTreeNode> GetLeafNodesInFrustum(Plane[] frustumPlanes, List<QuadTreeNode> result = null)
        {
            result ??= new List<QuadTreeNode>();
            CollectLeafNodesInFrustum(frustumPlanes, result);
            return result;
        }

        // ── 내부 재귀 헬퍼 ───────────────────────────────────────────────

        /// <summary>AABB 교차 기반 Leaf 노드 수집 (Orthographic 전용)</summary>
        private void CollectLeafNodesInAABB(AABB viewAABB, List<QuadTreeNode> result)
        {
            // 현재 노드가 뷰 영역과 겹치지 않으면 가지치기
            if (!Boundary.Intersects(viewAABB)) return;

            if (!Divided)
            {
                // Leaf 노드 — 뷰와 겹치므로 수집
                result.Add(this);
                return;
            }

            // 내부 노드 — 자식으로 재귀
            NW.CollectLeafNodesInAABB(viewAABB, result);
            NE.CollectLeafNodesInAABB(viewAABB, result);
            SW.CollectLeafNodesInAABB(viewAABB, result);
            SE.CollectLeafNodesInAABB(viewAABB, result);
        }

        /// <summary>Frustum Plane 기반 Leaf 노드 수집 (Perspective / 범용)</summary>
        private void CollectLeafNodesInFrustum(Plane[] planes, List<QuadTreeNode> result)
        {
            // 현재 노드의 AABB 를 3D Bounds 로 변환해 Frustum 교차 검사
            Bounds bounds = AABBToBounds(Boundary);
            if (!GeometryUtility.TestPlanesAABB(planes, bounds)) return;

            if (!Divided)
            {
                result.Add(this);
                return;
            }

            NW.CollectLeafNodesInFrustum(planes, result);
            NE.CollectLeafNodesInFrustum(planes, result);
            SW.CollectLeafNodesInFrustum(planes, result);
            SE.CollectLeafNodesInFrustum(planes, result);
        }

        // ── 정적 변환 유틸리티 ────────────────────────────────────────────

        /// <summary>
        /// 카메라를 월드 공간 AABB 로 변환합니다.
        /// XZ 평면 (y = 0) 기준이며, AABB 의 Vector2 는 (x, z) 에 대응합니다.
        ///
        /// ▸ Orthographic : 카메라 right/forward 의 XZ 성분으로 4개 코너를 계산합니다.
        ///   카메라가 수평 회전(Y축)되어 있어도 올바른 보수적 AABB 가 생성됩니다.
        /// ▸ Perspective  : 니어 플레인 ~ 파 플레인 범위를 XZ 로 투영한 AABB 를 반환합니다.
        ///   (GetLeafNodesInFrustum 의 Plane[] 오버로드가 더 정밀하므로 권장합니다.)
        /// </summary>
        private static AABB CameraToAABB(Camera cam)
        {
            if (cam.orthographic)
            {
                float   h   = cam.orthographicSize;
                float   w   = h * cam.aspect;
                Vector3 pos = cam.transform.position;

                // XZ 평면 기준: right → X축,  forward → Z축
                Vector3 right   = cam.transform.right   * w;   // (x, ?, z)
                Vector3 forward = cam.transform.forward * h;   // (x, ?, z)

                // 4개 코너의 XZ 성분
                Vector2 c0 = new Vector2(pos.x - right.x - forward.x, pos.z - right.z - forward.z);
                Vector2 c1 = new Vector2(pos.x + right.x - forward.x, pos.z + right.z - forward.z);
                Vector2 c2 = new Vector2(pos.x - right.x + forward.x, pos.z - right.z + forward.z);
                Vector2 c3 = new Vector2(pos.x + right.x + forward.x, pos.z + right.z + forward.z);

                float minX = Mathf.Min(c0.x, c1.x, c2.x, c3.x);
                float maxX = Mathf.Max(c0.x, c1.x, c2.x, c3.x);
                float minZ = Mathf.Min(c0.y, c1.y, c2.y, c3.y);
                float maxZ = Mathf.Max(c0.y, c1.y, c2.y, c3.y);

                Vector2 center   = new Vector2((minX + maxX) * 0.5f, (minZ + maxZ) * 0.5f);
                Vector2 halfSize = new Vector2((maxX - minX) * 0.5f, (maxZ - minZ) * 0.5f);
                return new AABB(center, halfSize);
            }
            else
            {
                // Perspective: 파 플레인 4 코너의 XZ 를 감싸는 AABB
                // (정밀도가 필요하면 GetLeafNodesInFrustum(Plane[]) 사용 권장)
                float   far     = cam.farClipPlane;
                float   halfFOV = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
                float   halfH   = far * Mathf.Tan(halfFOV);
                float   halfW   = halfH * cam.aspect;

                Vector3 pos     = cam.transform.position;
                Vector3 right   = cam.transform.right   * halfW;
                Vector3 forward = cam.transform.forward * far;

                Vector3 fl = pos + forward - right;   // far-left
                Vector3 fr = pos + forward + right;   // far-right

                float minX = Mathf.Min(pos.x, fl.x, fr.x);
                float maxX = Mathf.Max(pos.x, fl.x, fr.x);
                float minZ = Mathf.Min(pos.z, fl.z, fr.z);
                float maxZ = Mathf.Max(pos.z, fl.z, fr.z);

                Vector2 center   = new Vector2((minX + maxX) * 0.5f, (minZ + maxZ) * 0.5f);
                Vector2 halfSize = new Vector2((maxX - minX) * 0.5f, (maxZ - minZ) * 0.5f);
                return new AABB(center, halfSize);
            }
        }

        /// <summary>
        /// AABB (XZ 평면, y=0) → UnityEngine.Bounds 변환.
        /// Vector2.x → Bounds.x,  Vector2.y → Bounds.z,  Y 축은 float.MaxValue 로 열어둡니다.
        /// </summary>
        private static Bounds AABBToBounds(AABB aabb)
            => new Bounds(
                new Vector3(aabb.Center.x,   0f,              aabb.Center.y),
                new Vector3(aabb.HalfSize.x * 2f, float.MaxValue, aabb.HalfSize.y * 2f));

        // ─────────────────────────────────────────
        //  Gizmo 디버그 (Editor / Scene 뷰)
        // ─────────────────────────────────────────
#if UNITY_EDITOR
        /// <summary>Scene 뷰에서 쿼드트리 경계선 시각화 (OnDrawGizmos 에서 호출) — XZ 평면 (y=0)</summary>
        public void DrawGizmos(Color boundaryColor = default, Color pointColor = default)
        {
            if (boundaryColor == default) boundaryColor = Color.green;
            if (pointColor    == default) pointColor    = Color.red;

            Gizmos.color = boundaryColor;
            // XZ 평면: AABB.Center.x → X,  AABB.Center.y → Z,  Y = 0
            Gizmos.DrawWireCube(
                new Vector3(Boundary.Center.x,   0f, Boundary.Center.y),
                new Vector3(Boundary.HalfSize.x * 2f, 0f, Boundary.HalfSize.y * 2f));

            Gizmos.color = pointColor;
            foreach (var p in _points)
                Gizmos.DrawSphere(new Vector3(p.Position.x, 0f, p.Position.y), 0.1f);

            if (Divided)
            {
                NW.DrawGizmos(boundaryColor, pointColor);
                NE.DrawGizmos(boundaryColor, pointColor);
                SW.DrawGizmos(boundaryColor, pointColor);
                SE.DrawGizmos(boundaryColor, pointColor);
            }
        }
#endif

        // ─────────────────────────────────────────
        //  내부 헬퍼
        // ─────────────────────────────────────────
        private void Subdivide()
        {
            var (nwC, neC, swC, seC) = Boundary.QuadrantCenters();
            var qHalf = Boundary.HalfSize * 0.5f;

            NW = new QuadTreeNode(new AABB(nwC, qHalf), _capacity, Depth + 1);
            NE = new QuadTreeNode(new AABB(neC, qHalf), _capacity, Depth + 1);
            SW = new QuadTreeNode(new AABB(swC, qHalf), _capacity, Depth + 1);
            SE = new QuadTreeNode(new AABB(seC, qHalf), _capacity, Depth + 1);
            Divided = true;

            // 기존 포인트 재분배
            foreach (var p in _points)
            {
                if      (!NE.Insert(p))
                if      (!NW.Insert(p))
                if      (!SE.Insert(p))
                         SW.Insert(p);   // 마지막은 반환값 무시
            }
            _points.Clear();
        }

        private bool CouldContainNearer(Vector2 target, float bestSqrDist)
        {
            float dx = Mathf.Max(0, Mathf.Abs(target.x - Boundary.Center.x) - Boundary.HalfSize.x);
            float dy = Mathf.Max(0, Mathf.Abs(target.y - Boundary.Center.y) - Boundary.HalfSize.y);
            return dx * dx + dy * dy < bestSqrDist;
        }

        private QuadTreeNode[] SortChildrenByDistance(Vector2 target)
        {
            var children = new QuadTreeNode[] { NW, NE, SW, SE };
            Array.Sort(children, (a, b) =>
                (a.Boundary.Center - target).sqrMagnitude
                    .CompareTo((b.Boundary.Center - target).sqrMagnitude));
            return children;
        }
    }


    // ─────────────────────────────────────────────
    //  레이어 페이로드 (FillGridJitter / FillPoissonDisk layerTag 전용)
    // ─────────────────────────────────────────────
    /// <summary>
    /// layerTag 를 지정해 FillGridJitter / FillPoissonDisk 를 호출하면
    /// QTPoint.Data 에 이 타입이 자동으로 래핑됩니다.
    ///
    ///   if (pt.Data is LayerPoint lp)
    ///   {
    ///       Debug.Log(lp.Tag);      // "city" / "town" / "hamlet" …
    ///       Debug.Log(lp.UserData); // 원래 data 인자
    ///   }
    /// </summary>
    public sealed class LayerPoint
    {
        /// <summary>레이어 식별 태그 (FillGridJitter 의 layerTag 인자와 동일)</summary>
        public string Tag      { get; }

        /// <summary>호출자가 넘긴 원본 페이로드</summary>
        public object UserData { get; }

        public LayerPoint(string tag, object userData = null)
        {
            Tag      = tag;
            UserData = userData;
        }

        public override string ToString() => $"LayerPoint(tag:\"{Tag}\")";
    }


    // ─────────────────────────────────────────────
    //  거리 범위 버킷
    // ─────────────────────────────────────────────

    /// <summary>
    /// 하나의 거리 구간 (MinExclusive, MaxInclusive] 과 그 구간에 속한 포인트 목록.
    /// MinExclusive == 0 인 첫 버킷은 [0, MaxInclusive] 로 취급합니다.
    /// </summary>
    public sealed class DistanceBucket
    {
        /// <summary>구간 하한 (제외). 첫 버킷은 0.</summary>
        public float MinExclusive { get; }

        /// <summary>구간 상한 (포함). 마지막 버킷은 float.MaxValue (무한대).</summary>
        public float MaxInclusive { get; }

        /// <summary>이 구간에 속한 포인트 목록 (삽입 순).</summary>
        public IReadOnlyList<QTPoint> Points => _points;

        private readonly List<QTPoint> _points = new List<QTPoint>();

        internal DistanceBucket(float minExclusive, float maxInclusive)
        {
            MinExclusive = minExclusive;
            MaxInclusive = maxInclusive;
        }

        /// <summary>distance 가 이 버킷 구간에 포함되는지 확인합니다.</summary>
        public bool Contains(float distance)
        {
            bool aboveMin = MinExclusive == 0f
                ? distance >= 0f
                : distance >  MinExclusive;
            return aboveMin && distance <= MaxInclusive;
        }

        /// <summary>이 버킷 구간이 [rangeMin, rangeMax] 와 겹치는지 확인합니다.</summary>
        public bool Overlaps(float rangeMin, float rangeMax)
        {
            float effectiveMin = MinExclusive == 0f ? 0f : MinExclusive;
            return effectiveMin <= rangeMax && MaxInclusive >= rangeMin;
        }

        internal void Add(QTPoint point)    => _points.Add(point);
        internal bool Remove(QTPoint point) => _points.Remove(point);
        internal void Clear()               => _points.Clear();

        public override string ToString() =>
            MinExclusive == 0f
                ? $"DistanceBucket[0 ~ {MaxInclusive}] ({_points.Count} pts)"
                : MaxInclusive == float.MaxValue
                    ? $"DistanceBucket({MinExclusive} ~ ∞] ({_points.Count} pts)"
                    : $"DistanceBucket({MinExclusive} ~ {MaxInclusive}] ({_points.Count} pts)";
    }


    /// <summary>
    /// 거리 구간 버킷 테이블.
    /// QuadTree 생성 시 thresholds 를 넘기면 자동 생성됩니다.
    ///
    /// 예) thresholds = [10, 20, 50]  →  버킷 [0~10], (10~20], (20~50], (50~∞]
    /// </summary>
    public sealed class DistanceBucketTable
    {
        //private readonly DistanceBucket[] _buckets;

        List<DistanceBucket> _buckets = new List<DistanceBucket>();

        /// <summary>모든 버킷의 읽기 전용 목록.</summary>
        public IReadOnlyList<DistanceBucket> Buckets => _buckets;

        /// <summary>버킷 수.</summary>
        public int Count => _buckets.Count;

        float bucketSize;

        //internal DistanceBucketTable(float[] thresholds)
        internal DistanceBucketTable( float bucket_size , float Max_Val ) 
        {
            // if (thresholds == null || thresholds.Length == 0)
            //     throw new ArgumentException("thresholds must have at least one value.");

            // // 정렬 + 중복 제거
            // var sorted = new List<float>(thresholds);
            // sorted.Sort();
            // var unique = new List<float>();
            // foreach (var t in sorted)
            // {
            //     if (t > 0f && (unique.Count == 0 || !Mathf.Approximately(t, unique[unique.Count - 1])))
            //         unique.Add(t);
            // }
            // if (unique.Count == 0)
            //     throw new ArgumentException("thresholds must contain at least one positive value.");

            // // 버킷 생성: [0~t0], (t0~t1], … , (tN~∞]
            // _buckets = new DistanceBucket[unique.Count + 1];
            // _buckets[0] = new DistanceBucket(0f, unique[0]);
            // for (int i = 1; i < unique.Count; i++)
            //     _buckets[i] = new DistanceBucket(unique[i - 1], unique[i]);
            // _buckets[unique.Count] = new DistanceBucket(unique[unique.Count - 1], float.MaxValue);

            bucketSize = bucket_size;

            int cur_step = 0;
            _buckets.Clear();
            while( true )
            {
                float s = cur_step * bucket_size;
                float e = (cur_step + 1) * bucket_size;

                if( Max_Val < s )break;

                DistanceBucket node =  new DistanceBucket(s, e);
                _buckets.Add(node);

                cur_step++;

                if(  cur_step > 10000 )
                {
                    break;
                }
            }

        }

        // ── 내부 조작 ────────────────────────────────────────────────

        /// <summary>distance 에 해당하는 버킷에 포인트를 추가합니다.</summary>
        internal void Add(QTPoint point, float distance)
        {
            var bucket = FindBucket(distance);

            // csj 추가
            point.BucketDist = distance;
            point.OnEnterBucketDist();

            bucket?.Add(point);
        }

        /// <summary>포인트를 포함하는 모든 버킷에서 제거합니다.</summary>
        internal void Remove(QTPoint point)
        {
            foreach (var b in _buckets) b.Remove(point);
        }

        /// <summary>모든 버킷을 비웁니다.</summary>
        internal void Clear()
        {
            foreach (var b in _buckets) b.Clear();
        }

        // ── 공개 조회 ────────────────────────────────────────────────

        /// <summary>
        /// distance 가 속하는 버킷 1개를 반환합니다.
        /// 어떤 구간에도 속하지 않으면 null 을 반환합니다.
        /// </summary>
        public DistanceBucket GetBucket(float distance)
            => FindBucket(distance);

        /// <summary>
        /// [rangeMin, rangeMax] 범위와 겹치는 버킷 목록을 반환합니다.
        /// </summary>
        public List<DistanceBucket> GetBuckets(float rangeMin, float rangeMax)
        {
            if (rangeMin > rangeMax)
                (rangeMin, rangeMax) = (rangeMax, rangeMin);

            var result = new List<DistanceBucket>();

            // foreach (var b in _buckets)
            //     if (b.Overlaps(rangeMin, rangeMax))
            //         result.Add(b);
            int s_step = (int)(rangeMin / bucketSize);
            int e_step = (int)(rangeMax / bucketSize);

            for( int i = s_step ; i < e_step+1 ; i++ )
            {
                result.Add( _buckets[i] );
            }
            return result;
        }

        /// <summary>
        /// [rangeMin, rangeMax] 범위에 속한 모든 포인트를 하나의 목록으로 반환합니다.
        /// </summary>
        public List<QTPoint> GetPoints(float rangeMin, float rangeMax)
        {
            var buckets = GetBuckets(rangeMin, rangeMax);
            var result  = new List<QTPoint>();
            foreach (var b in buckets)
                result.AddRange(b.Points);
            return result;
        }

        // ── 내부 헬퍼 ────────────────────────────────────────────────
        private DistanceBucket FindBucket(float distance)
        {
            // foreach (var b in _buckets)
            //     if (b.Contains(distance)) return b;
            // return null;

            int cur_step = (int)(distance / bucketSize);

            if( _buckets.Count > cur_step )
                return _buckets[ cur_step ];

            return null;
        }
    }


    // ─────────────────────────────────────────────
    //  QuadTree 래퍼 (편의 클래스)
    // ─────────────────────────────────────────────
    public class QuadTree
    {
        private QuadTreeNode         _root;
        private readonly int         _capacity;
        private DistanceBucketTable  _bucketTable;   // null = 거리 버킷 미사용

        public int TotalCount => _root.Count();
        public int MaxDepth   => _root.MaxDepth();

        public QuadTreeNode Root => _root;

        /// <summary>
        /// 거리 버킷 테이블. QuadTree 생성 시 distanceThresholds 를 지정했을 때만 non-null.
        /// </summary>
        public DistanceBucketTable BucketTable => _bucketTable;

        /// <summary>거리 버킷 없이 기본 쿼드트리만 생성합니다.</summary>
        public QuadTree(AABB boundary, int capacity = 4)
        {
            _capacity    = capacity;
            _root        = new QuadTreeNode(boundary, capacity);
            _bucketTable = null;
        }

        /// <summary>
        /// 거리 버킷을 포함한 쿼드트리를 생성합니다.
        /// distanceThresholds 예) new float[]{ 10, 20, 50, 100 }
        /// → 버킷 [0~10], (10~20], (20~50], (50~100], (100~∞] 가 생성됩니다.
        /// </summary>
        ///public QuadTree(AABB boundary, float[] distanceThresholds, int capacity = 4)
         public QuadTree(AABB boundary, float bucket_size , int capacity = 4)
        {
            _capacity    = capacity;
            _root        = new QuadTreeNode(boundary, capacity);

            _bucketTable = new DistanceBucketTable(bucket_size , boundary.HalfSize.x * 2 );
        }

        // ── 기본 CRUD ──────────────────────────────

        /// <summary>포인트를 트리에 삽입합니다. distanceFromOrigin 을 지정하면 버킷에도 등록됩니다.</summary>
        public bool Insert(QTPoint point, float distanceFromOrigin = -1f)
        {
            bool ok = _root.Insert(point);
            if (ok && _bucketTable != null)
            {
                // 버킷이 있으면 거리를 계산 , 인자는 무시
                float d = Vector2.Distance( _root.Boundary.Center  , point.Position ); 

                // float d = distanceFromOrigin >= 0f
                //     ? distanceFromOrigin
                //     : point.Position.magnitude;          // 기본값: 원점(0,0)까지 거리
                _bucketTable.Add(point, d);
            }
            return ok;
        }

        /// <summary>포인트를 트리에 삽입합니다. distanceFromOrigin 을 지정하면 버킷에도 등록됩니다.</summary>
        public bool Insert(Vector2 pos, int _tag_hash , object data = null, float distanceFromOrigin = -1f)
        {
            var pt = new QTPoint(pos,_tag_hash , data);
            return Insert(pt, distanceFromOrigin);
        }

        /// <summary>포인트를 트리와 버킷 모두에서 제거합니다.</summary>
        public bool Remove(QTPoint point)
        {
            bool ok = _root.Remove(point);
            if (ok) _bucketTable?.Remove(point);
            return ok;
        }

        /// <summary>트리와 버킷을 모두 초기화합니다.</summary>
        public void Clear()
        {
            _root.Clear();
            _bucketTable?.Clear();
        }

        // ── 조회 ───────────────────────────────────
        public List<QTPoint> QueryRect(AABB range , int _tag_hash = 0)
                                                  => _root.Query(range , _tag_hash);
        public List<QTPoint> QueryRect(Vector2 center, Vector2 halfSize , int _tag_hash = 0)
                                                  => _root.Query(new AABB(center, halfSize) , _tag_hash);
        public List<QTPoint> QueryCircle(Vector2 center, float radius , int _tag_hash = 0)
                                                  => _root.QueryCircle(center, radius , _tag_hash );

        // ── 거리 버킷 조회 ────────────────────────────

        /// <summary>
        /// distance 가 속하는 버킷 1개를 반환합니다.
        /// 버킷 테이블이 없으면 null 을 반환합니다.
        /// </summary>
        public DistanceBucket GetBucket(float distance)
            => _bucketTable?.GetBucket(distance);

        /// <summary>
        /// [rangeMin, rangeMax] 에 겹치는 버킷 목록을 반환합니다.
        /// 버킷 테이블이 없으면 빈 목록을 반환합니다.
        /// </summary>
        public List<DistanceBucket> GetBuckets(float rangeMin, float rangeMax)
            => _bucketTable?.GetBuckets(rangeMin, rangeMax) ?? new List<DistanceBucket>();

        /// <summary>
        /// [rangeMin, rangeMax] 에 속한 모든 포인트를 하나의 목록으로 반환합니다.
        /// 버킷 테이블이 없으면 빈 목록을 반환합니다.
        /// </summary>
        public List<QTPoint> GetPointsByDistance(float rangeMin, float rangeMax)
            => _bucketTable?.GetPoints(rangeMin, rangeMax) ?? new List<QTPoint>();

        // ── 최근접 이웃 ────────────────────────────────
        /// <summary>가장 가까운 이웃 반환</summary>
        public QTPoint FindNearest(Vector2 target , string tag = "")
        {
            float bestDist = float.MaxValue;
            int tag_hash = 0;
            if( string.IsNullOrEmpty(tag) == false )tag_hash = tag.GetHashCode();
            return _root.FindNearest(target, ref bestDist , tag_hash);
        }

        /// <summary>K개의 가장 가까운 이웃 반환</summary>
        public List<QTPoint> FindKNearest(Vector2 target, int k , int _tag_hash )
                                                  => _root.FindKNearest(target, k , _tag_hash);

        public List<QTPoint> GetAllPoints(int _tag_hash) => _root.GetAllPoints(_tag_hash);

        // ── 재구성 (모든 점을 새 트리로 재삽입) ────
        public void Rebuild()
        {
            var all  = GetAllPoints(0);
            var root = _root;
            _root    = new QuadTreeNode(root.Boundary, _capacity);
            _bucketTable?.Clear();
            foreach (var p in all) Insert(p);   // Insert 오버로드로 버킷도 재등록
        }

        // ── 카메라 시야 내 Leaf 노드 수집 ────────────
        /// <summary>
        /// 2D 직교(Orthographic) 카메라 시야 안에 있는 최하위(Leaf) 노드를 모두 반환합니다.
        /// </summary>
        /// <param name="camera">기준 카메라</param>
        /// <returns>시야 내 Leaf 노드 목록</returns>
        public List<QuadTreeNode> GetLeafNodesInView(Camera camera)
            => _root.GetLeafNodesInView(camera);

        /// <summary>
        /// Perspective 카메라 또는 임의 절두체 기준으로 최하위(Leaf) 노드를 모두 반환합니다.
        /// <code>
        ///   Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        ///   var leaves = tree.GetLeafNodesInFrustum(planes);
        /// </code>
        /// </summary>
        public List<QuadTreeNode> GetLeafNodesInFrustum(Plane[] frustumPlanes)
            => _root.GetLeafNodesInFrustum(frustumPlanes);

        // ─────────────────────────────────────────
        //  포인트 생성 알고리즘
        // ─────────────────────────────────────────

        /// <summary>
        /// Bridson's Poisson Disk Sampling 으로 포인트를 생성해 트리에 누적 삽입합니다.
        /// 쿼드트리의 QueryCircle 을 충돌 검사에 활용하므로 이웃 탐색이 O(log n) 수준입니다.
        ///
        /// ── 레이어 누적 배치 ──────────────────────────────────────────
        /// layerTag 를 지정하면 트리 내 모든 포인트(이전 레이어 포함)를 현재 minDistance 로
        /// 검사합니다. 새 레이어가 기존 포인트와 겹치는 것을 방지하면서도
        /// 레이어별로 독립적인 간격 기준을 적용할 수 있습니다.
        ///
        ///   // 대도시: 서로 80 이상 유지
        ///   qt.FillPoissonDisk(80f, 10,  data: city, layerTag: "city");
        ///   // 소도시: 소도시끼리·대도시와도 30 이상 유지
        ///   qt.FillPoissonDisk(30f, 50,  data: town, layerTag: "town");
        /// </summary>
        /// <param name="minDistance">포인트 간 최소 거리 (r)</param>
        /// <param name="maxCount">삽입할 최대 포인트 수. 0 이하이면 공간이 포화될 때까지 생성.</param>
        /// <param name="candidatesPerPoint">활성 포인트당 후보 샘플 수 (기본 30, 클수록 촘촘하지만 느림)</param>
        /// <param name="data">포인트에 붙일 사용자 페이로드. layerTag 사용 시 LayerPoint.UserData 로 접근합니다.</param>
        /// <param name="layerTag">레이어 식별 문자열. 같은 태그끼리만 거리 충돌 검사를 수행합니다.</param>
        /// <returns>새로 삽입된 QTPoint 목록</returns>
        public List<QTPoint> FillPoissonDisk(
            float  minDistance,
            int    maxCount           = 0,
            int    candidatesPerPoint = 30,
            object data               = null,
            string layerTag           = null)
        {
            if (minDistance <= 0f)
                throw new ArgumentException("minDistance must be > 0", nameof(minDistance));

            var     inserted = new List<QTPoint>();
            var     active   = new List<Vector2>();

            // 이번 호출에서 삽입한 포인트만 모아두는 집합 (레이어 필터용)
            var     layerSet = layerTag != null ? new HashSet<QTPoint>() : null;

            AABB    bounds   = _root.Boundary;
            float   sqrMin   = minDistance * minDistance;

            int tag_hash = layerTag != null ? layerTag.GetHashCode() : 0;

            // ── 씨앗 포인트 생성 헬퍼 ────────────────────────────────
            object MakePayload() =>
                layerTag != null ? new LayerPoint(layerTag, data) : data;

            // ── 첫 씨앗 포인트 ────────────────────────────────────────
            Vector2 seed   = new Vector2(

                // UnityEngine.Random.Range(bounds.Left,   bounds.Right),
                // UnityEngine.Random.Range(bounds.Bottom, bounds.Top)
                RandomFloat(bounds.Left,   bounds.Right),
                RandomFloat(bounds.Bottom, bounds.Top)
                );

            var seedPt = new QTPoint(seed, tag_hash , MakePayload());
            Insert(seedPt);                     // 트리 + 버킷 동시 등록
            inserted.Add(seedPt);
            layerSet?.Add(seedPt);
            active.Add(seed);

            // ── 메인 루프 ─────────────────────────────────────────────
            while (active.Count > 0)
            {
                if (maxCount > 0 && inserted.Count >= maxCount) break;

                // 활성 목록에서 무작위 기준점 선택
                //int     baseIdx = UnityEngine.Random.Range(0, active.Count);
                int     baseIdx = RandomInt(0, active.Count);
                Vector2 basePos = active[baseIdx];
                bool    found   = false;

                for (int i = 0; i < candidatesPerPoint; i++)
                {
                    if (maxCount > 0 && inserted.Count >= maxCount) break;

                    // r ~ 2r 환형 영역에서 후보 샘플

                    // float   angle     = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                    // float   dist      = UnityEngine.Random.Range(minDistance, minDistance * 2f);
                    float   angle     = RandomFloat(0f, Mathf.PI * 2f);
                    float   dist      = RandomFloat(minDistance, minDistance * 2f);

                    Vector2 candidate = new Vector2(
                        basePos.x + Mathf.Cos(angle) * dist,
                        basePos.y + Mathf.Sin(angle) * dist);

                    if (!bounds.Contains(candidate)) continue;

                    // ── 거리 충돌 검사 ──────────────────────────────────
                    // 트리 내 모든 포인트(이전 레이어 포함)를 현재 minDistance 로 검사합니다.
                    // layerSet 은 활성 목록 관리(같은 레이어 씨앗 확장)용으로만 사용합니다.
                    bool tooClose = false;
                    foreach (var nb in _root.QueryCircle(candidate, minDistance))
                    {
                        if ((nb.Position - candidate).sqrMagnitude < sqrMin)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (tooClose) continue;

                    //var newPt = new QTPoint(candidate, MakePayload());
                    var newPt = CreatePoint(candidate, tag_hash , MakePayload());
                    Insert(newPt);               // 트리 + 버킷 동시 등록
                    inserted.Add(newPt);
                    layerSet?.Add(newPt);
                    active.Add(candidate);
                    found = true;
                    break;
                }

                // k 번 모두 실패 → 활성 목록에서 제거
                if (!found)
                    active.RemoveAt(baseIdx);
            }

            return inserted;
        }


        /// <summary>
        /// Grid Jitter 방식으로 포인트를 생성해 트리에 누적 삽입합니다.
        /// 격자 간격 = minDistance × gridSpacing 으로 격자를 구성하고,
        /// 각 셀 중심에 ±jitterRatio 범위의 랜덤 오프셋을 적용합니다.
        /// maxCount 편향을 줄이기 위해 격자 셀 순서를 무작위로 섞어 순회합니다.
        ///
        /// ── 레이어 누적 배치 ──────────────────────────────────────────
        /// layerTag 를 지정하면 트리 내 모든 포인트(이전 레이어 포함)를 현재 minDistance 로
        /// 검사합니다. 덕분에 새 레이어 포인트가 기존 포인트와 겹치거나 너무 가깝게
        /// 배치되는 것을 막으면서도, 레이어별로 독립적인 간격 기준을 적용할 수 있습니다.
        ///
        ///   // 대도시: 서로 80 이상 유지
        ///   qt.FillGridJitter(80f, 10,  data: city,   layerTag: "city");
        ///   // 소도시: 소도시끼리·대도시와도 30 이상 유지
        ///   qt.FillGridJitter(30f, 50,  data: town,   layerTag: "town");
        ///   // 마을:   마을끼리·앞 두 레이어와도 10 이상 유지
        ///   qt.FillGridJitter(10f, 200, data: hamlet, layerTag: "hamlet");
        /// </summary>
        /// <param name="minDistance">셀 크기의 기준이 되는 최소 거리 (r)</param>
        /// <param name="maxCount">삽입할 최대 포인트 수. 0 이하이면 제한 없음.</param>
        /// <param name="gridSpacing">셀 간격 배율 (기본 1.0, 클수록 성기게)</param>
        /// <param name="jitterRatio">셀 크기 대비 지터 비율 0 ~ 0.5 (기본 0.35)</param>
        /// <param name="data">포인트에 붙일 사용자 페이로드 (선택). layerTag 사용 시 LayerPoint.UserData 로 접근합니다.</param>
        /// <param name="layerTag">레이어 식별 문자열. 같은 태그끼리만 거리 충돌 검사를 수행합니다.</param>
        /// <returns>새로 삽입된 QTPoint 목록</returns>
        public List<QTPoint> FillGridJitter(
            float  minDistance,
            int    maxCount    = 0,
            float  gridSpacing = 1.0f,
            float  jitterRatio = 0.35f,
            object data        = null,
            string layerTag    = null
            // ,string layerTag_exc = null 
            // ,float exc_range = 0 
              )
        {
            if (minDistance <= 0f)
                throw new ArgumentException("minDistance must be > 0", nameof(minDistance));

            jitterRatio = Mathf.Clamp(jitterRatio, 0f, 0.5f);

            var   inserted = new List<QTPoint>();

            // 이번 호출에서 방금 삽입한 포인트만 모아두는 집합.
            // QueryCircle 결과에서 "같은 레이어" 여부를 O(1) 로 판별합니다.
            var   layerSet = layerTag != null ? new HashSet<QTPoint>() : null;

            int tag_hash = layerTag != null ? layerTag.GetHashCode() : 0;

            //int tag_hash_exc = layerTag_exc != null ? layerTag.GetHashCode() : 0;

            AABB  bounds   = _root.Boundary;
            float step     = minDistance * Mathf.Max(gridSpacing, 0.5f);
            float jitter   = step * jitterRatio;
            float sqrMin   = minDistance * minDistance;

            int cols = Mathf.Max(1, Mathf.FloorToInt(bounds.HalfSize.x * 2f / step));
            int rows = Mathf.Max(1, Mathf.FloorToInt(bounds.HalfSize.y * 2f / step));

            // 격자 전체를 무작위 순서로 순회해 maxCount 편향 최소화
            var indices = new List<int>(cols * rows);
            for (int i = 0; i < cols * rows; i++) indices.Add(i);
            ShuffleList(indices , this);

            foreach (int idx in indices)
            {
                if (maxCount > 0 && inserted.Count >= maxCount) break;

                int   col = idx % cols;
                int   row = idx / cols;

                float cx = bounds.Left   + (col + 0.5f) * step;
                float cy = bounds.Bottom + (row + 0.5f) * step;

                Vector2 candidate = new Vector2(
                    
                    // cx + UnityEngine.Random.Range(-jitter, jitter),
                    // cy + UnityEngine.Random.Range(-jitter, jitter)

                    cx + RandomFloat(-jitter, jitter),
                    cy + RandomFloat(-jitter, jitter)
                    );

                if (!bounds.Contains(candidate)) continue;

                // ── 거리 충돌 검사 ──────────────────────────────────────
                // 트리 내 모든 포인트(이전 레이어 포함)를 현재 minDistance 로 검사합니다.
                // → 새 레이어가 기존 포인트와 너무 가깝게 배치되는 것을 방지합니다.
                // layerSet 은 "이번 호출에서 삽입한 포인트" 식별용으로만 사용합니다.
                bool tooClose = false;
                foreach (var nb in _root.QueryCircle(candidate, minDistance))
                {
                    if ((nb.Position - candidate).sqrMagnitude < sqrMin)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) continue;

                // 페이로드: layerTag 사용 시 LayerPoint 로 래핑해 나중에 태그 식별 가능하게 함
                object payload = layerTag != null ? new LayerPoint(layerTag, data) : data;

                // // 제외 해시태그
                // if( tag_hash_exc != 0 )
                // {
                //     List<QTPoint> qTs_exc = _root.QueryCircle( candidate , exc_range , tag_hash_exc );
                //     if( qTs_exc.Count > 0 ) continue;
                // }

                //var pt = new QTPoint(candidate, payload);
                var pt = CreatePoint(candidate, tag_hash , payload);
                if (Insert(pt))                 // 트리 + 버킷 동시 등록
                {
                    inserted.Add(pt);
                    layerSet?.Add(pt);  // 이번 레이어 집합에 등록
                }
            }

            return inserted;
        }

        // ── 내부 헬퍼 ─────────────────────────────────────────────────
        /// <summary>Fisher-Yates 셔플 (FillGridJitter 격자 순서 무작위화에 사용)</summary>
        private static void ShuffleList<T>(List<T> list , QuadTree quad)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                //int j = UnityEngine.Random.Range(0, i + 1);
                int j = quad.RandomInt(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        // 델리게이트 , 포인트 생성
        public delegate QTPoint Dele_CreatePoint(Vector2 position, int _tag_hash, object data = null);
        public Dele_CreatePoint dele_CreatePoint;
        public QTPoint CreatePoint( Vector2 position, int _tag_hash , object data = null )
        {
            if( dele_CreatePoint != null ) return dele_CreatePoint(position, _tag_hash , data);
            return new QTPoint(position, _tag_hash , data);
        }

        // 랜덤 함수
        public delegate int Dele_RandomInt(int min , int max);
        public Dele_RandomInt dele_RandomInt;
        public int RandomInt( int min , int max )
        {
            if( dele_RandomInt != null ) return dele_RandomInt( min , max );
            return UnityEngine.Random.Range(min, max);
        }


        public delegate float Dele_RandomFloat(float min , float max);
        public Dele_RandomFloat dele_RandomFloat;
        public float RandomFloat( float min , float max )
        {
            if( dele_RandomFloat != null ) return dele_RandomFloat( min , max );
            return UnityEngine.Random.Range(min, max);
        }

        // 일정 영역에서 랜덤 포인트를 해보고 인자로 넘긴 거리보다 큰거 쿼리
        public bool CreateTempPointDist( Vector2 vCenter , float half , float minDist , ref Vector2 ref_pos , int try_num = 500 )
        {
            AABB aABB = new AABB(vCenter.x,vCenter.y ,half+minDist,half+minDist );

            float sqrMin = minDist * minDist;

            List<QTPoint> src_q = _root.Query(aABB);
            bool tooClose = false;
            for( int i = 0 ; i < try_num ; i++ )
            {
                Vector2 candidate = new Vector2();

                candidate.x = RandomFloat( aABB.Center.x - aABB.HalfSize.x , aABB.Center.x + aABB.HalfSize.x );
                candidate.y = RandomFloat( aABB.Center.y - aABB.HalfSize.y , aABB.Center.y + aABB.HalfSize.y );
                tooClose = false;
                foreach (var nb in src_q )
                {
                    if ((nb.Position - candidate).sqrMagnitude < sqrMin)
                    {
                        tooClose = true;
                        break;
                    }
                }                
                if( tooClose == false )
                {
                    ref_pos = candidate;
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        public void DrawGizmos(Color boundaryColor = default, Color pointColor = default)
            => _root.DrawGizmos(boundaryColor, pointColor);
#endif
    }
}
