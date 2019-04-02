using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
    public bool SnapX { get { return snapX; } set { snapX = value; } }
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;

    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;
    private RectTransform baseRect = null;

    private Canvas canvas;
    private Camera cam;

    private Vector2 input = Vector2.zero;

    protected virtual void Start()
    {
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
        StartCoroutine("Firing");
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public float fireRate = 0.0f;
    private float nextFire = 0.0f;
    private Vector2 bulletPos;
    public GameObject BulletToRight, BulletToLeft;
    private GameObject bullet;

    public void OnDrag(PointerEventData eventData)
    {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, cam);
        handle.anchoredPosition = input * radius * handleRange;
        // We set the direction for player movement to the vector of the joystick movement
        //Debug.Log(input);
        if (gameObject.name == "MovementStick")
        {
            PlayerScript.StickDirection = input;
        }
        else
        {
            // Fire weapon
            if (Time.time > nextFire)
            {
                if (BulletToRight == null || BulletToLeft == null)
                {
                    BulletToRight = GameObject.FindGameObjectWithTag("BulletToRight");
                    BulletToLeft = GameObject.FindGameObjectWithTag("BulletToLeft");
                }

                isFiring = true;
                if (bullet != null) nextFire = Time.time + FireRate;
            }

            // Rotate weapon
            if (input.x != 0.0f || input.y != 0.0f)
            {
                float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
                GameObject weapon = GameObject.FindGameObjectWithTag("Weapon");
                int rotationAdjust = 85;
                if (PlayerScript.FacingRight)
                {
                    angle = -angle + rotationAdjust;
                }
                else
                {
                    angle = -angle - rotationAdjust;
                }
                weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }
    }

    public static int MaxAmmo = 8;
    public static float FireRate = 0.5f;
    public static int AmmoCount = MaxAmmo;
    private bool isReloading = false;

    IEnumerator Firing()
    {
        while (true)
        {
            if (isFiring && Time.time > nextFire && AmmoCount>0)
            {
                Debug.Log("Firing");
                bulletPos = GameObject.FindGameObjectWithTag("Player").transform.position;
                bullet = Instantiate(BulletToRight, bulletPos, Quaternion.identity);
                AmmoCount--;
                GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>().text = $"Ammo: {AmmoCount}/{MaxAmmo}";
                nextFire = Time.time + FireRate;
                int bulletForce = bullet.gameObject.GetComponent<BulletScript>().BulletForce;
                bullet.gameObject.GetComponent<Rigidbody2D>().AddForce(input.normalized * bulletForce, ForceMode2D.Impulse);
            }

            if (AmmoCount == 0 && !isReloading)
            {
                StartCoroutine("Reload");
            }

            if (!isReloading) GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>().text = $"Ammo: {AmmoCount}/{MaxAmmo}";
            yield return new WaitForSeconds(0.005f);
        }
    }

    IEnumerator Reload()
    {
        if (!isReloading)
        {
            isReloading = true;
            Debug.Log("Reloading");
            GameObject.FindGameObjectWithTag("StatusText").GetComponent<Text>().text = "Reloading!";
            GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.one;
            GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>().text = $"Reloading!";
            yield return new WaitForSeconds(bullet.gameObject.GetComponent<BulletScript>().ReloadSpeed);
            AmmoCount = MaxAmmo;
            isReloading = false;
            GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>().text = $"Ammo: {AmmoCount}/{MaxAmmo}";
            GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.zero;
            Debug.Log("Reload complete");
        }
    }

    private bool isFiring = false;

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                input = normalised;
        }
        else
            input = Vector2.zero;
    }

    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        }
        else
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        // We set the direction for player movement to zero vector
        if (gameObject.name == "MovementStick")
        {
             PlayerScript.StickDirection = Vector2.zero;
        }
        else
        {
            // Stop firing
            isFiring = false;
        }

    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
            return localPoint - (background.anchorMax * baseRect.sizeDelta);
        return Vector2.zero;
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }