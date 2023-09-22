using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Block : MonoBehaviour
{

    public static float density = 0.3f;
    public static float keyChance = 0.05f;
    public static float shiftTime = 0.1f;

    public Tile wallTile;
    public GameObject keyPrefab;
    public Vector3Int gridPos;

    private Grid grid;
    private Tilemap tilemap;


    private void Awake()
    {
        grid = GetComponent<Grid>();
        tilemap = GetComponentInChildren<Tilemap>();

        gridPos = LevelController.instance.grid.WorldToCell(transform.position);
    }
    private void Start()
    {
        Generate();
    }
    public void Generate()
    {
        Vector3Int[] compass = { Vector3Int.right, Vector3Int.down, Vector3Int.up, Vector3Int.left };
        foreach (Vector3Int dir in compass)
        {
            if (Random.Range(0f, 1f) < density)
            {
                tilemap.SetTile(new Vector3Int(1, 1) + dir, wallTile);
            }
        }
        if (Random.Range(0f, 1f) <= keyChance)
        {
            Instantiate(keyPrefab, grid.CellToWorld(new Vector3Int(1, 1)) + grid.cellSize / 2, Quaternion.identity, transform);
        }
    }
    public void Shift(Vector3Int shift)
    {
        gridPos += shift;
        StopCoroutine(ShiftRoutine());
        StartCoroutine(ShiftRoutine());
    }
    IEnumerator ShiftRoutine()
    {
        float startTime = Time.time;
        Vector3 targetPos = LevelController.instance.grid.CellToWorld(gridPos);
        Vector3 startPos = transform.position;
        while (Time.time < startTime + shiftTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, (Time.time - startTime) / shiftTime);
            yield return null;
        }
        transform.position = targetPos;
    }
    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }
    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(shiftTime);
        Destroy(gameObject);
    }
}