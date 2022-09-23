
using UnityEngine;
using TMPro;

/// Thanks for downloading my projectile gun script! :D
/// Feel free to use it in any project you like!
/// 
/// The code is fully commented but if you still have any questions
/// don't hesitate to write a yt comment
/// or use the #coding-problems channel of my discord server
/// 
/// Dave
public class CajadoScript : MonoBehaviour
{
  //bullet 
  public GameObject bullet;

  //bullet force


  //Gun stats
  public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;

  //bools
  bool shooting, readyToShoot, reloading;

  //Reference

  public Transform attackPoint;

  //Graphics

  //bug fixing :D
  public bool allowInvoke = true;

  private void Awake()
  {
    //make sure magazine is full
    readyToShoot = true;
  }

  private void Update()
  {
    Atirar();
    // MyInput();

    // //Set ammo display, if it exists :D
    // if (ammunitionDisplay != null)
    //   ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
  }
  public void Atirar()
  {
    if (readyToShoot && !reloading)
    {
      Shoot();
    }
  }
  public void Shoot()
  {
    readyToShoot = false;

    //Instantiate bullet/projectile
    GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
                                                                                               //Rotate bullet to shoot direction
                                                                                               //Add forces to bullet
    currentBullet.GetComponent<Rigidbody>().AddForce(10 * transform.forward, ForceMode.Impulse);


    if (allowInvoke)
    {
      Invoke("ResetShot", timeBetweenShooting);
      allowInvoke = false;
    }
  }
  private void ResetShot()
  {
    //Allow shooting and invoking again
    allowInvoke = true;
    readyToShoot = true;
  }

  private void Reload()
  {
    reloading = true;
    Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
  }
  private void ReloadFinished()
  {
    //Fill magazine
    reloading = false;
  }
}
