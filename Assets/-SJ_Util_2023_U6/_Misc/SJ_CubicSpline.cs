using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_CubicSpline
{
    public int key_gap = 500;
    public int limit_count = 3;

    public List<Vector3> list_pos = new List<Vector3>();

    List<float> x_series = new List<float>();
    List<float> h = new List<float>();
    List<Vector3> alpha = new List<Vector3>();
    List<float> l = new List<float>();
    List<float> u = new List<float>();
    List<Vector3> z = new List<Vector3>();
    List<Vector3> c = new List<Vector3>();
    List<Vector3> b = new List<Vector3>();
    List<Vector3> d = new List<Vector3>();

    public  void    Clear()
    {
        list_pos.Clear();
    }

	public	Vector3		GetLastPos()
	{
		return list_pos[list_pos.Count-1];
	}

    public  void    Add_Pos( Vector3 pos )
    {
        list_pos.Add(pos);
        if (list_pos.Count < limit_count) return;
        if (list_pos.Count > limit_count) list_pos.RemoveAt(0);

        x_series.Clear();
        h.Clear();
        alpha.Clear();
        l.Clear();
        u.Clear();
        z.Clear();
        c.Clear();
        b.Clear();
        d.Clear();


        int n = limit_count - 1;

        for (int i = 0; i < n + 1; i++)
        {
            h.Add(0);
            alpha.Add(Vector3.zero);
            x_series.Add( i * key_gap );
        }


        for ( int i = 0; i <= n - 1; i++)
        {
            h[i] = key_gap;
        }


        for (int i = 1; i <= n - 1; i++)
        {
            alpha[i] = 3 * (list_pos[i + 1] - list_pos[i]) / h[i] - 3 * (list_pos[i] - list_pos[i - 1]) / h[i - 1];
        }

        for(int i = 0; i < n + 1; i++ )
        {
            l.Add(0);
            u.Add(0);
            z.Add(Vector3.zero);
            c.Add(Vector3.zero);
            b.Add(Vector3.zero);
            d.Add(Vector3.zero);
        }
        l[0] = 1; u[0] = 0; z[0] = Vector3.zero;

        for ( int i = 1; i <= n - 1; i++)
        {
            l[i] = 2 * (x_series[i + 1] - x_series[i - 1]) - h[i - 1] * u[i - 1];
            u[i] = h[i] / l[i];
            z[i] = (alpha[i] - h[i - 1] * z[i - 1]) / l[i];
        }


        l[n] = 1; z[n] = Vector3.zero; c[n] = Vector3.zero;
        for (int i = n - 1; i >= 0; i--)
        {
            Vector3 p;
            p.x = z[i].x - u[i] * c[i + 1].x;
            p.y = z[i].y - u[i] * c[i + 1].y;
            p.z = z[i].z - u[i] * c[i + 1].z;

            c[i] = p;
            b[i] = (list_pos[i + 1] - list_pos[i]) / h[i] - h[i] * (c[i + 1] + 2 * c[i]) / 3;
            d[i] = (c[i + 1] - c[i]) / (3 * h[i]);
        }

    }


    public  bool     Get_LastPosLerp( float r , ref Vector3 ref_vec )
    {
        if (list_pos.Count < limit_count) return false;

        int i = limit_count - 2;

        float x = x_series[i];
        float inc = (x_series[i + 1] - x_series[i]) * r;

        x += inc;
        float x_offset = x - x_series[i];

        Vector3 Sx = list_pos[i] + b[i] * x_offset + c[i] * x_offset * x_offset + d[i] * x_offset * x_offset * x_offset;
        ref_vec = Sx;
        return true;
    }

}
